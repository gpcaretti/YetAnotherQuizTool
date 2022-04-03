// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.
// Write your JavaScript code.
$(document).ready(function () {

	var CurrentExamId = null;

	var QnA = {};
	var CurrentQId = null;
	var CurrentQidx = -1;

	var QnAResults = [];

	var Score = null;
	var Status = null;

	var objReport = null;

	var CheckTime = [];

	var constraints = { audio: true, video: { width: { min: 640, ideal: 640, max: 640 }, height: { min: 480, ideal: 480, max: 480 }, framerate: 60 } };
	var recBtn = document.querySelector('button#btnStart');
	var stopBtn = document.querySelector('button#btnSubmit');
	var liveVideoElement = document.querySelector('#gum');   

	$('#ddlExam').prop('disabled', false);
	$('#btnStart').prop('disabled', false);
	$('#btnSubmit').prop('disabled', false);
	$('#btnSave').prop('disabled', true);
	$('#eqMain button.w3-left').prop('disabled', true);
	$('#eqMain button.w3-right').prop('disabled', true);
	$("#eqReport").children().prop('disabled', true);
	$('#eqReport a').removeAttr("href");
	$('#eqReport i').addClass("w3-opacity-max");
	$("#eqScore").children().prop('disabled', true);    

	var mediaRecorder;
	var chunks = [];
	var count = 0;
	var localStream = null;
	var soundMeter = null;
	var containerType = "video/webm"; //defaults to webm but we switch to mp4 on Safari 14.0.2+

	if (!navigator.mediaDevices.getUserMedia) {
		alert('navigator.mediaDevices.getUserMedia not supported on your browser, use the latest version of Firefox or Chrome');
	} else {
		if (window.MediaRecorder == undefined) {
			alert('MediaRecorder not supported on your browser, use the latest version of Firefox or Chrome');
		} else {
			navigator.mediaDevices.getUserMedia(constraints)
				.then(function (stream) {
					localStream = stream;

					localStream.getTracks().forEach(function (track) {
						if (track.kind == "audio") {
							track.onended = function (event) {
								console.log("audio track.onended Audio track.readyState=" + track.readyState + ", track.muted=" + track.muted);
							}
						}
						if (track.kind == "video") {
							track.onended = function (event) {
								log("video track.onended Audio track.readyState=" + track.readyState + ", track.muted=" + track.muted);
							}
						}
					});

					liveVideoElement.srcObject = localStream;
					liveVideoElement.play();

					try {
						window.AudioContext = window.AudioContext || window.webkitAudioContext;
						window.audioContext = new AudioContext();
					} catch (e) {
						console.log('Web Audio API not supported.');
					}

					soundMeter = window.soundMeter = new SoundMeter(window.audioContext);
					soundMeter.connectToSource(localStream, function (e) {
						if (e) {
							console.log(e);
							return;
						} else {
							/*setInterval(function() {
							   log(Math.round(soundMeter.instant.toFixed(2) * 100));
						   }, 100);*/
						}
					});

				}).catch(function (err) {
					/* handle the error */
					console.log('navigator.getUserMedia error: ' + err);
				});
		}
	}

	// fetch available exams
	$.ajax({
		type: "GET",
		url: "/api/Exams",
		data: "{}",
		success: function (data) {
			var string = '<option value="">--- Please Select ---</option>';
			//let tabs = "";
			for (var i = 0; i < data.length; i++) {
				let item = data[i];
				let shortName = item.name	// get the text within the div
					.trim()					// remove leading and trailing spaces
					.substring(0, 40);		// get first N characters
				if (shortName.length < item.name.length)
					shortName = shortName
					.split(" ")				// separate characters into an array of words
					.slice(0, -1)			// remove the last full or partial word
					.join(" ") + "...";		// combine into a single string and append "..."
				let tabs = (!!item.ancestorId) ? "&nbsp;&nbsp;" : "";
				string += '<option value="' + item.id + '">' + tabs + shortName + '</option>';
			}
			$("#ddlExam").html(string);
		}
	});

	/**
	 * Click to fetch, init and start an exam based on the selected exam
	 */
	$('#btnStart').click(function () {
		if (!!$("#ddlExam").val()) {
			FetchExam($("#ddlExam").val());
		} else {
			$.alert({
				icon: 'fa fa-warning',
				type: 'orange',
				title: 'Select Skill',
				content: 'Please select your skill !',
				boxWidth: '40%',
				useBootstrap: false,
				closeIcon: true,
				closeIconClass: 'fa fa-close'
			});
		}
	});

	/**
	 * Click previous question button
	 */
	$('#btnPrev').click(function () {
		let idx = (CurrentQidx - 1) % QnA.totalCount;
		if (idx <= QnA.totalCount - 1) {
			// save current choice of the user
			$('#btnSave').click();
			// change the print the previous question 
			MoveToQuestionAndPrint(idx);
		}
	});

	/**
	 * Click next question button
	 */
	$('#btnNext').click(function () {
		let idx = (CurrentQidx + 1) % QnA.totalCount;
		if (idx <= QnA.totalCount - 1) {
			// save current choice of the user
			$('#btnSave').click();
			// change the print the next question
			MoveToQuestionAndPrint(idx);
		}
	});

	/**
	 * Click 'save answer/choice' to current question
	 */
	$('#btnSave').click(function () {
		let answer = {
			candidateId: $('#eqCandidateId').text(),
			examId: CurrentExamId,
			questionId: CurrentQId,
			choiceId: $('input[name="option"]:checked').val() || null,
		};
		answer.isCorrect = !!answer.choiceId && (answer.choiceId == QnA.questions.find(item => item.id == answer.questionId)?.correctChoiceId)

		let idx = QnAResults.findIndex(item => item.questionId === CurrentQId);
		if (idx >= 0) {
			QnAResults[idx] = answer;
			//UpdateItem(CurrentQId);
		}
		else {
			QnAResults.push(answer);
		}       
	});

	$('#btnSubmit').click(function () {              
		$.confirm({
			icon: 'fa fa-warning',
			title: 'Submit Quiz',
			content: 'Are you sure you want to submit the quiz ?',
			type: 'orange',
			closeIcon: true,
			closeIconClass: 'fa fa-close',
			boxWidth: '40%',
			useBootstrap: false,
			buttons: {
				Submit: {
					text: 'Submit',
					btnClass: 'btn-red',
					action: function () {
						// save current choice of the user
						$('#btnSave').click();
						// now post the results of the quiz
						$.post('/api/Score/', { objRequest: QnAResults },
						 function (data) {
							 if (data > 0) {
								 StopTimer();
								 StopRecord();
								 $('#btnSubmit').prop('disabled', true);
								 $("#eqReport").children().prop('disabled', false);
								 $("#eqReport a").attr("href", "/Score/Result");
								 $('#eqReport i').removeClass("w3-opacity-max");
								 $.alert({
									 type: 'green',
									 title: 'Success !',
									 content: 'Please check the score.',
									 boxWidth: '40%',
									 useBootstrap: false,
									 closeIcon: true,
									 closeIconClass: 'fa fa-close'
								 });
							 }
							 else {
								 $('#btnSubmit').prop('disabled', false);
								 $("#eqReport").children().prop('disabled', true);
								 $('#eqReport a').removeAttr("href");
								 $('#eqReport i').addClass("w3-opacity-max");
								 $.alert({
									 type: 'red',
									 title: 'Error !',
									 content: 'Please try again.',
									 boxWidth: '40%',
									 useBootstrap: false,
									 closeIcon: true,
									 closeIconClass: 'fa fa-close'
								 });
							 }
						 });
					}
				},
				Cancel: function () {
					$(this).remove();
				}
			}
		});
	});

	$('.btnScore').click(function () {
		var request = {
			ExamId: $(this).closest("tr").find('td:eq(2)').text(),
			CandidateId: $('#hdnCandidateId').val(),            
			SessionId: $(this).closest("tr").find('td:eq(1)').text()            
		};
		Score = $(this).closest("tr").find('td:eq(4)').text();
		Status = $(this).closest("tr").find('td:eq(6)').text();
		$.post('/api/Report/', { argRpt: request },
			function (data) {
				objReport = data;
				$('div#eqScore h3').html(data[0].exam + ' Test');
				$('div#eqScore .w3-container p:eq(0)').html('<strong>Candidate ID:</strong> ' + data[0].candidateID);
				$('div#eqScore .w3-container h5').html(data[0].message);
				$('div#eqScore .w3-container span').html('<strong>Date:</strong> ' + data[0].date);
				if (Status == "1") {
					$("#eqScore").children().prop('disabled', false);
				}
				else {
					Score = null;
					objReport = null;
					$("#eqScore").children().prop('disabled', true);
				}
			});
	});

	$('#btnReport').click(function () {
		//console.log(objReport);
		var scoreFormat = {
			ExamId: objReport[0].examId,
			CandidateId: $('#hdnCandidateId').val(),
			SessionId: objReport[0].sessionID,
			Exam: objReport[0].exam,
			Date: objReport[0].date,
			Score: Score
		};
		//console.log(scoreFormat);
		$.post('/api/CreatePDF/', { argPDFRpt:scoreFormat},
		   function (data) {
				//console.log(data);
				if (data.isSuccess = true) { window.open(data.path, '_blank'); }
		   });       
	});

	$('#chooseFile').change(function () {
	  var file = $('#chooseFile')[0].files[0].name;
	  $('#noFile').text(file);
	});


	/**
	 * 
	 * @param {number} qIdx
	 */
	function MoveToQuestionAndPrint(qIdx) {
		if ((qIdx < 0) || (qIdx >= QnA.totalCount)) {
			DisplayErrorAlert(null, `Index out of range (${qIdx}). Cannot select the question.`);
			return;
		}
		let question = QnA.questions[qIdx];
		CurrentQidx = qIdx;
		CurrentQId = question.id;

		// print the question
		$('div#eqMain p').empty();
		$('#eqCount').html(`(${qIdx + 1} of ${QnA.totalCount})`);
		$('div#eqMain h3').html(QnA.examName);
		$('div#eqMain h4').html(`${(question.code || qIdx + 1)}: ${question.statement}`);

		// print the possible choices. If the user previously selected one of them, check it
		let choiceSelectedByUser = QnAResults.find(o => o.questionId === CurrentQId);
		let oString = "<div style='padding: 5px;' id='eqOption'>";
		for (let i in question.choices) {
			let choice = question.choices[i];
			let isSelected = choice.id == (choiceSelectedByUser?.choiceId ?? 0);
			oString += `<label><input class='w3-radio' type='radio' name='option' value='${choice.id}' ${(isSelected ? 'checked' : '')}> ${choice.statement}</label><br/>`;
		}
		oString += "</div>";
		$('div#eqMain p').append(oString);

		// enable/disable prev & next btns
		$('#eqMain button.w3-left').prop('disabled', qIdx <= 0);
		$('#eqMain button.w3-right').prop('disabled', (qIdx + 1) >= QnA.totalCount);
	}

	/**
	 * Fetch and exam and its question. Then start tuner and recording
	 * @param {number} examId
	 */
	function FetchExam(examId) {
		CurrentExamId = null;

		// enable/disable proper buttons
		$('#ddlExam').prop('disabled', true);
		$('#btnStart').prop('disabled', true);
		$('#btnSave').prop('disabled', false);

		$.get('/api/Exam/', { ExamId: examId },
			(data, textStatus, jqXHR) => {
				CurrentExamId = examId;	// save the exam Id
				FetchQuestions(examId);
				StartTimer(data.duration || 0);
				StartRecord();
			}).fail((jqXHR, textStatus) => {
				$('#ddlExam').prop('disabled', false);
				$('#btnStart').prop('disabled', false);
				$('#btnSave').prop('disabled', true);
				//let errorData = $.parseJSON(jqXHR.responseText);
				//let errorJson = jqXHR.responseJSON;
				let errorJson = jqXHR.responseJSON || { error: 'Server error', exception: jqXHR.responseText };
				if (errorJson.exception) console.error(errorJson.exception | errorJson.error);
				DisplayErrorAlert(errorJsonm, textStatus, jqXHR.status);
			});
	}

	/**
	 * Fetch question and print the first one
	 * @param {any} examId
	 */
	function FetchQuestions(examId) {
		$.get('/api/Questions', { examId: examId, isRecursive: true, isRandom: true, maxResultCount: 50 },
			(data) => {
				QnA = data;
				MoveToQuestionAndPrint(0);
			}).fail(function (jqXHR, textStatus) {
				//let errorData = $.parseJSON(jqXHR.responseText);
				//let errorJson = jqXHR.responseJSON;
				let errorJson = jqXHR.responseJSON || { error: 'Server error', exception: jqXHR.responseText };
				if (errorJson.exception) console.error(errorJson.exception | errorJson.error);
				DisplayErrorAlert(errorJson, textStatus, jqXHR.status);
			});
	}

	/**
	 * Start a timer to update the countdown
	 * @param {number} durationMin
	 */
	function StartTimer(durationMin) {
		let deadline = new Date(new Date().getTime() + durationMin * 60000);
		if (CheckTime.length == 0) {
			var x = setInterval(() => {
				let now = new Date().getTime();
				let deltaT = deadline.getTime() - now;
				let hours = Math.floor((deltaT % (1000 * 60 * 60 * 24)) / (1000 * 60 * 60));
				let minutes = Math.floor((deltaT % (1000 * 60 * 60)) / (1000 * 60));
				let seconds = Math.floor((deltaT % (1000 * 60)) / 1000);
				document.getElementById("timer").innerHTML = "Time : " + hours + ":" + minutes + ":" + seconds;
				if (deltaT < 0) {
					clearInterval(x);
					document.getElementById("timer").innerHTML = "Time : 00:00:00";
				}
			}, 1000);
			CheckTime.push(x);            
		}
	}

	function StopTimer() {
		clearInterval(CheckTime[0]);
		CheckTime = [];       
	}

	//Recording
	function StartRecord() {
		if (localStream == null) {
			alert('Could not get local stream from mic/camera');
		} else {            
			chunks = [];
			/* use the stream */
			console.log('Start recording...');
			if (typeof MediaRecorder.isTypeSupported == 'function') {
				/*
					MediaRecorder.isTypeSupported is a function announced in https://developers.google.com/web/updates/2016/01/mediarecorder and later introduced in the MediaRecorder API spec http://www.w3.org/TR/mediastream-recording/
				*/
				if (MediaRecorder.isTypeSupported('video/webm;codecs=vp9')) {
					var options = { mimeType: 'video/webm;codecs=vp9' };
				} else if (MediaRecorder.isTypeSupported('video/webm;codecs=h264')) {
					var options = { mimeType: 'video/webm;codecs=h264' };
				} else if (MediaRecorder.isTypeSupported('video/webm')) {
					var options = { mimeType: 'video/webm' };
				} else if (MediaRecorder.isTypeSupported('video/mp4')) {
					//Safari 14.0.2 has an EXPERIMENTAL version of MediaRecorder enabled by default
					containerType = "video/mp4";
					var options = { mimeType: 'video/mp4' };
				}
				console.log('Using ' + options.mimeType);
				mediaRecorder = new MediaRecorder(localStream, options);
			} else {
				console.log('isTypeSupported is not supported, using default codecs for browser');
				mediaRecorder = new MediaRecorder(localStream);
			}

			mediaRecorder.ondataavailable = function (e) {
				console.log('mediaRecorder.ondataavailable, e.data.size=' + e.data.size);
				if (e.data && e.data.size > 0) {
					chunks.push(e.data);
				}
			};

			mediaRecorder.onerror = function (e) {
				console.log('mediaRecorder.onerror: ' + e);
			};

			mediaRecorder.onstart = function () {
				console.log('mediaRecorder.onstart, mediaRecorder.state = ' + mediaRecorder.state);

				localStream.getTracks().forEach(function (track) {
					if (track.kind == "audio") {
						console.log("onstart - Audio track.readyState=" + track.readyState + ", track.muted=" + track.muted);
					}
					if (track.kind == "video") {
						console.log("onstart - Video track.readyState=" + track.readyState + ", track.muted=" + track.muted);
					}
				});
			};

			mediaRecorder.onstop = function () {
				console.log('mediaRecorder.onstop, mediaRecorder.state = ' + mediaRecorder.state);

				//var recording = new Blob(chunks, {type: containerType});
				var recording = new Blob(chunks, { type: mediaRecorder.mimeType });
				PostBlob(recording);               
			};

			mediaRecorder.onpause = function () {
				console.log('mediaRecorder.onpause, mediaRecorder.state = ' + mediaRecorder.state);
			}

			mediaRecorder.onresume = function () {
				console.log('mediaRecorder.onresume, mediaRecorder.state = ' + mediaRecorder.state);
			}

			mediaRecorder.onwarning = function (e) {
				console.log('mediaRecorder.onwarning: ' + e);
			};
		   
			mediaRecorder.start(1000);

			localStream.getTracks().forEach(function (track) {
				console.log(track.kind + ":" + JSON.stringify(track.getSettings()));
				console.log(track.getSettings());
			})
		}
	}

	function StopRecord() {
		mediaRecorder.stop();
		liveVideoElement.srcObject = null;
	}

	function PostBlob(blob) {
		var formData = new FormData();
		formData.append('video-blob', blob);
		$.ajax({
			type: 'POST',
			url: "/Home/SaveRecoredFile",
			data: formData,
			cache: false,
			contentType: false,
			processData: false,
			success: function (result) {
			   if (result) {
				  console.log('Success');
			   }
			},
			error: function (result) {
				console.log(result);
			}
		});
	}

	function DisplayErrorAlert(errorJson, messageText, statusCode) {
		$.alert({
			icon: 'fa fa-error',
			type: 'red',
			title: errorJson?.error || "Error",
			content: "Error: " + messageText + " (" + errorJson?.error + ")<br/>Status: " + statusCode,
			boxWidth: '40%',
			useBootstrap: false,
			closeIcon: true,
			closeIconClass: 'fa fa-close'
		});
	}
});

//Image Upload Preview  
function ShowImagePreview(input) {
   if (input.files && input.files[0]) {
		var reader = new FileReader();
		reader.onload = function (e) {
			$('#imgCandidate').prop('src', e.target.result);
		};
		reader.readAsDataURL(input.files[0]);
   }
}






