import { Component, OnInit, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
    selector: 'app-quiz',
    templateUrl: './quiz.component.html',
    styleUrls: ['./quiz.component.css']
})
export class QuizComponent implements OnInit {
    title: string;
    selectedQuiz: Quiz;
    quizzes: Quiz[];

    constructor(
        http: HttpClient,
        @Inject('BASE_URL') baseUrl: string) {
        this.title = "Latest Quizzes";
        var url = baseUrl + "api/quiz/Latest/";

        http.get<Quiz[]>(url).subscribe(result => {
            this.quizzes = result;
        }, error => console.error(error));
    }

    ngOnInit() {
    }
    onSelect(quiz: Quiz) {
        this.selectedQuiz = quiz;
        console.log("quiz with Id "
            + this.selectedQuiz.Id
            + " has been selected.");
    }

}
