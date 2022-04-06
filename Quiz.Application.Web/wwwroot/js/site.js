// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.
// Write your JavaScript code.
$(document).ready(function () {

	var ExamSession = {};
	var ExamSessionResults = {};

	var CurrentQId = null;
	var CurrentQidx = -1;

	var Score = null;
	var Status = null;

	var objReport = null;

	var CountDownTimer = null;

	var constraints = { audio: true, video: { width: { min: 640, ideal: 640, max: 640 }, height: { min: 480, ideal: 480, max: 480 }, framerate: 60 } };
	var liveVideoElement = document.querySelector('#gum');   

	$('#ddlExam').prop('disabled', false);
	$('#btnStart').prop('disabled', false);
	$('#btnSubmit').prop('disabled', false);

	//$('#btnSave').prop('disabled', true);
	//$('#btnShowHideSession').prop('disabled', true);
	//$('#btnEndSession').prop('disabled', true);
	//$('#btnRestartSession').prop('disabled', true);
	$('#eqMain button.w3-left').prop('disabled', true);
	$('#eqMain button.w3-right').prop('disabled', true);

	$("#eqReport").children().prop('disabled', true);
	$('#eqReport a').removeAttr("href");
	$('#eqReport i').addClass("w3-opacity-max");
	$("#eqScore").children().prop('disabled', true);    

	var mediaRecorder;
	var chunks = [];
	var count = 0;
	var LocalStream = null;
	var SoundMeter = null;
	var containerType = "video/webm"; //defaults to webm but we switch to mp4 on Safari 14.0.2+

// TODO: media recording skipped by GP!
/*
	if (!navigator.mediaDevices.getUserMedia) {
		alert('navigator.mediaDevices.getUserMedia not supported on your browser, use the latest version of Firefox or Chrome');
	} else {
		if (window.MediaRecorder == undefined) {
			alert('MediaRecorder not supported on your browser, use the latest version of Firefox or Chrome');
		} else {
			navigator.mediaDevices.getUserMedia(constraints)
				.then(function (stream) {
					LocalStream = stream;

					LocalStream.getTracks().forEach(function (track) {
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

					liveVideoElement.srcObject = LocalStream;
					liveVideoElement.play();

					try {
						window.AudioContext = window.AudioContext || window.webkitAudioContext;
						window.audioContext = new AudioContext();
					} catch (e) {
						console.log('Web Audio API not supported.');
					}

					SoundMeter = window.soundMeter = new SoundMeter(window.audioContext);
					SoundMeter.connectToSource(LocalStream, function (e) {
						if (e) {
							console.log(e);
							return;
						} else {
						   // setInterval(function() {
						   //    log(Math.round(soundMeter.instant.toFixed(2) * 100));
						   //}, 100);
						}
					});

				}).catch(function (err) {
					// handle the error
					console.log('navigator.getUserMedia error: ' + err);
				});
		}
	}
*/

	// fetch available exams
	$.ajax({
		type: "GET",
		url: "/api/Exams",
		data: "{}",
		success: function (availableExams) {
			var string = '<option value="">--- Please Select ---</option>';
			let tabs = [];
			let lastAncestorId = null;
			let sorted = availableExams.sort((elem1, elem2) => (elem1.code > elem2.code) ? +1 : (elem1.code < elem2.code) ? -1 : 0);
			for (let i in sorted) {
				let item = availableExams[i];
				let shortName = item.name	// get the text within the div
					.trim()					// remove leading and trailing spaces
					.substring(0, 50);		// get first N characters
				if (shortName.length < item.name.length)
					shortName = shortName
						.split(" ")				// separate characters into an array of words
						.slice(0, -1)			// remove the last full or partial word
						.join(" ") + "...";		// combine into a single string and append "..."
				if (!item.ancestorId) {
					tabs = [];
				} else if (tabs.length == 0) {
					tabs.push("&nbsp;&nbsp;&nbsp;");
				}
				string += `<option value="${item.id}">${tabs.join('')} + ${shortName}</option>`;
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

	$('#btnMoveToStart').click(function () {
		SaveUserAnswerAndMoveTo(0);
	});

	/**
	 * Click previous question button
	 */
	$('#btnPrev').click(function () {
		SaveUserAnswerAndMoveTo(CurrentQidx - 1);
	});

	/**
	 * Click next question button
	 */
	$('#btnNext').click(function () {
		SaveUserAnswerAndMoveTo(CurrentQidx + 1);
	});

	$('#btnMoveToEnd').click(function () {
		SaveUserAnswerAndMoveTo(ExamSession.totalCount - 1);
	});

	/**
	 * Click 'save answer/choice' to current question
	 */
	$('#btnSave').click(function () {
		SaveUserAnswer();
	});

	$('#btnShowHideSession').click(() => {
		SaveUserAnswer();
		ExamSessionStopStartToggle();
		$('#btnShowHideSession').html(ExamSessionResults.isEnded ? "Hide solutions" : "Show solutions");
	});

	$('#btnEndSession').click(() => {
		EndExamSession();
	});

	$('#btnRestartSession').click(() => {
		StartExamSession();
	});

	/**
	 *
	 */
	$('#btnSubmit').click(function () {              
		$.confirm({
			icon: 'fa fa-warning',
			title: 'Submit Quiz',
			content: 'Are you sure you want to submit the quiz?',
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
						SaveUserAnswer();
						// now post the results of the quiz
						$.post('/api/Score/', { objRequest: ExamSessionResults },
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
	 */
	$('#btnStopContinueTimer').click(function () {
		if (!ExamSession) return;
		if (!CountDownTimer) {
			ContinueTimer();
			$('#btnStopContinueTimer').html("Pause timer");
		} else {
			StopTimer();
			$('#btnStopContinueTimer').html("Continue timer");
		}
	});


	/**
	 *
	 */
	function SaveUserAnswer() {
		let idx = ExamSessionResults.answers.findIndex(item => item.questionId === CurrentQId);
		let choiceId = $('input[name="option"]:checked').val() || null;
		if (!choiceId) {
			// user did not select a choice. Update the answer history and return
			if (idx >= 0) ExamSessionResults.answers.splice(idx, 1);
			return;
		}

		let answer = {
			//candidateId: $('#eqCandidateId').text(),
			examId: ExamSession.examId,
			questionId: CurrentQId,
			choiceId: choiceId,
			correctChoiceId: ExamSession.questions.find(item => item.id == CurrentQId)?.correctChoiceId
		};
		answer.isCorrect = !!answer.choiceId && (answer.choiceId == answer.correctChoiceId);

		if (idx >= 0) {
			ExamSessionResults.answers[idx] = answer;
			//UpdateItem(CurrentQId);
		}
		else {
			ExamSessionResults.answers.push(answer);
		}
	}

	/**
	 *
	 */
	function StartExamSession() {
		ExamSessionResults = {
			answers: [],
			isEnded: false
		};
		// enable/disable proper buttons
		UINewSessionControls(false);
		MoveToQuestionAndPrint(0);
		StartTimer(ExamSession.duration || 0);
		//TODO: StartRecord();
	}

	function EndExamSession() {
		ExamSessionResults.isEnded = true;
		UINewSessionControls(true);
		ClearQuestionArea();
		if (ExamSession.totalCount > 0) MoveToQuestionAndPrint(0);
		StopTimer();
		StopRecord();
	}

	function ExamSessionStopStartToggle() {
		ExamSessionResults.isEnded = !ExamSessionResults.isEnded;
		MoveToQuestionAndPrint(CurrentQidx);
		//UINewSessionControls(ExamSessionResults.isEnded);
	//	StopTimer();
	//	StopRecord();
	}

	function SaveUserAnswerAndMoveTo(idx) {
		// save current choice of the user
		SaveUserAnswer();
		idx = idx % ExamSession.totalCount;
		if (idx <= ExamSession.totalCount - 1) MoveToQuestionAndPrint(idx);
	}

	function ClearQuestionArea() {
		$('p#choices, div#eqMain h3, div#eqMain h4').empty();
	}

	/**
	 * 
	 * @param {number} qIdx
	 */
	function MoveToQuestionAndPrint(qIdx) {
		if ((qIdx < 0) || (qIdx >= ExamSession.totalCount)) {
			ShowErrorAlert(null, `Index out of range (${qIdx}). Cannot select the question.`);
			return;
		}

		let question = ExamSession.questions[qIdx];
		CurrentQidx = qIdx;
		CurrentQId = question.id;

		// print the question
		$('p#choices').empty();
		$('#eqCount').html(`(${qIdx + 1} of ${ExamSession.totalCount})`);
		$('div#eqMain h3').html(ExamSession.name);
		if (ExamSessionResults.isEnded) {
			let correct = ExamSessionResults.answers.reduce((acc, answ) => answ.isCorrect ? ++acc : acc, 0);
			let wrong = ExamSessionResults.answers.reduce((acc, answ) => !answ.isCorrect ? ++acc : acc, 0);
			let notAnswered = ExamSession.questions.length - correct - wrong;
			$('div#examSessionResults').html(
				`<span class='correctChoice'>${correct} correct answer(s)</span><br/>` +
				`<span class='wrongChoice'>${wrong} wrong answer(s)</span><br/>` +
				`<span class=''><b>${notAnswered} not answered</b></span>`);
		} else {
			$('div#examSessionResults').empty();
		}

		$('div#eqMain h4').html(`${(question.code || qIdx + 1)}: ${question.statement}`);

		// print the possible choices. If the user previously selected one of them, check it
		let choiceSelectedByUser = ExamSessionResults.answers.find(o => o.questionId === CurrentQId);
		let oString = "<div style='padding: 5px;' id='eqOption'>";
		for (let i in question.choices.sort((elem1, elem2) => elem1.position - elem2.position)) {
			let choice = question.choices[i];
			let checked = choice.id == (choiceSelectedByUser?.choiceId ?? 0) ? "checked" : "";
			let readonly = ExamSessionResults.isEnded ? "disabled readonly" : "";
			let colorMark = (readonly && checked && !choice.isCorrect)
				? "wrongChoice"
				: (readonly && !checked && choice.isCorrect)
					? "correctChoiceHighlighted"
					: (readonly && checked && choice.isCorrect)
						? "correctChoice"
						: "";
			oString += `<div class='${colorMark}'><label class=''><input class='w3-radio' type='radio' name='option' value='${choice.id}' ${checked} ${readonly}> ${choice.statement}</label></div>`;
		}
		oString += "</div>";
		$('p#choices').append(oString);

		// enable/disable prev & next btns
		$('#eqMain button.w3-left').prop('disabled', qIdx <= 0);
		$('#eqMain button.w3-right').prop('disabled', (qIdx + 1) >= ExamSession.totalCount);
	}

	/**
	 * Enable/disable proper buttons
	 * @param {boolean } enabled
	 */
	function UINewSessionControls(enabled) {
		$('#ddlExam').prop('disabled', !enabled);
		$('#btnStart').prop('disabled', !enabled);
		$('#btnSave').prop('disabled', !enabled);

	//	$('#btnShowHideSession').prop('disabled', !enabled);
	//	$('#btnEndSession').prop('disabled', !enabled);
	//	$('#btnRestartSession').prop('disabled', !enabled);
	}

	/**
	 * Fetch and exam and its question. Then start tuner and recording
	 * @param {number} examId
	 */
	function FetchExam(examId) {
		// enable/disable proper buttons
		UINewSessionControls(false);
		// fetch a new exam session
		$.get('/api/Exam/', { ExamId: examId },
			(exam, textStatus, jqXHR) => {
				try {
					PrepareExamSession(exam.id);
					StartExamSession();
				} catch (e) {
					EndExamSession();
					StopTimer();
					StopRecord();
				}
			}).fail((jqXHR, textStatus) => {
				UINewSessionControls(true);
				//let errorData = $.parseJSON(jqXHR.responseText);
				//let errorJson = jqXHR.responseJSON;
				let errorJson = jqXHR.responseJSON || { error: 'Server error', exception: jqXHR.responseText };
				if (errorJson.exception) console.error(errorJson.exception | errorJson.error);
				ShowErrorAlert(errorJsonm, textStatus, jqXHR.status);
			});
	}

	/**
	 * Fetch questions for a test session and print the first question
	 * @param {any} examId
	 */
	function PrepareExamSession(examId) {
		$.get('/api/PrepareExamSession', { examId: examId, isRecursive: true, isRandom: true, maxResultCount: 20 },
			(data) => {
				ExamSession = data;
				ExamSessionResults = {
					candidateId: $('#eqCandidateId').text(),
					examId: data.examId,
					answers: []
				};
				MoveToQuestionAndPrint(0);
			}).fail(function (jqXHR, textStatus) {
				//let errorData = $.parseJSON(jqXHR.responseText);
				//let errorJson = jqXHR.responseJSON;
				let errorJson = jqXHR.responseJSON || { error: 'Server error', exception: jqXHR.responseText };
				if (errorJson.exception) console.error(errorJson.exception | errorJson.error);
				ShowErrorAlert(errorJson, textStatus, jqXHR.status);
			});
	}

	/**
	 * Start a timer to update the countdown
	 * @param {number} durationMin
	 */
	function StartTimer(durationMin) {
		let deadline = new Date(new Date().getTime() + durationMin * 60000);
		DoStartTimer(deadline);
	}

	/**
	 * Continue current timer update the countdown
	 */
	function ContinueTimer() {
		let timeString = document.getElementById("timer").innerHTML;
		let idx = "Time: ".length;
		timeString = timeString.substring(idx);

		//innerHTML.substr("Time: ").length();
		timeString = document.getElementById("timer").innerHTML.substr("Time: ".length).trim();
		let deadline = AddTimeToDay(new Date(), timeString);
		DoStartTimer(deadline);
	}

	/**
	 * Stop the timer to update the countdown
	 */
	function StopTimer() {
		if (!!CountDownTimer) clearInterval(CountDownTimer);
		CountDownTimer = null;
	}

	function DoStartTimer(deadline) {
		StopTimer();
		CountDownTimer = setInterval(() => {
			let now = new Date().getTime();
			let deltaT = deadline.getTime() - now;
			let hours = Math.floor((deltaT % (1000 * 60 * 60 * 24)) / (1000 * 60 * 60));
			let minutes = Math.floor((deltaT % (1000 * 60 * 60)) / (1000 * 60));
			let seconds = Math.floor((deltaT % (1000 * 60)) / 1000);
			document.getElementById("timer").innerHTML = "Time : " + hours + ":" + minutes + ":" + seconds;
			if (deltaT < 0) {
				StopTimer();
				document.getElementById("timer").innerHTML = "Time : 00:00:00";
			}
		}, 1000);
	}

	/**
	 * Add the passed timeString to the passed date (only the date)
	 * 
	 * @param {Date} date
	 * @param {String} timeString - in the form of hh:mm:ss
	 */
	function AddTimeToDay(date, timeString) {
		var month = '' + (date.getMonth() + 1),
			day = '' + date.getDate(),
			year = date.getFullYear();
		if (month.length < 2) month = '0' + month;
		if (day.length < 2) day = '0' + day;

		return new Date(`${year}-${month}-${day}T${timeString}`);
	}


	// Recording
	function StartRecord() {
		if (LocalStream == null) {
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
				mediaRecorder = new MediaRecorder(LocalStream, options);
			} else {
				console.log('isTypeSupported is not supported, using default codecs for browser');
				mediaRecorder = new MediaRecorder(LocalStream);
			}

			mediaRecorder.ondataavailable = function (e) {
				//console.log('mediaRecorder.ondataavailable, e.data.size=' + e.data.size);
				if (e.data && e.data.size > 0) {
					chunks.push(e.data);
				}
			};

			mediaRecorder.onerror = function (e) {
				console.log('mediaRecorder.onerror: ' + e);
			};

			mediaRecorder.onstart = function () {
				console.log('mediaRecorder.onstart, mediaRecorder.state = ' + mediaRecorder.state);

				LocalStream.getTracks().forEach(function (track) {
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

			LocalStream.getTracks().forEach(function (track) {
				console.log(track.kind + ":" + JSON.stringify(track.getSettings()));
				console.log(track.getSettings());
			})
		}
	}

	function StopRecord() {
		if (!!mediaRecorder) mediaRecorder.stop();
		if (!!liveVideoElement) liveVideoElement.srcObject = null;
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

	function ShowErrorAlert(errorJson, messageText, statusCode) {
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






