using System;
using System.Collections.Generic;
using Quiz.Domain.Users;

namespace Quiz.Domain.Exams {
    //public class Option {
    //    public Guid CandidateId { get; set; }
    //    public Guid ExamId { get; set; }
    //    public Guid QuestionId { get; set; }
    //    public Guid AnswerId { get; set; }
    //    public Guid SelectedOption { get; set; }
    //}
    public class Root {
        public Candidate objCandidate { get; set; }
        public List<QuizAttempt> objAttempt { get; set; }
    }
    public class QuizAttempt {
        public Guid Id { get; set; }
        public string SessionId { get; set; }
        public Guid ExamId { get; set; }
        public string Exam { get; set; }
        public string Date { get; set; }
        public string Score { get; set; }
        public string Status { get; set; }
    }
    public class QuizReport {
        public Guid CandidateId { get; set; }
        public string SessionId { get; set; }
        public Guid ExamId { get; set; }
        public string Exam { get; set; }
        public string Date { get; set; }
        public string Message { get; set; }
    }
    public class ReqReport {
        public Guid ExamId { get; set; }
        public string CandidateId { get; set; }
        public string SessionId { get; set; }
    }
    public class ReqCertificate {
        public Guid CandidateId { get; set; }
        public string SessionId { get; set; }
        public Guid ExamId { get; set; }
        public string Exam { get; set; }
        public string Date { get; set; }
        public string Score { get; set; }
    }

    public class Request {
    }
    public class ResPDF {
        public bool IsSuccess { get; set; }
        public string Path { get; set; }
    }
}
