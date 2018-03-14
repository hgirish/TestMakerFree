import { Component, OnInit, Inject } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'quiz-edit',
  templateUrl: './quiz-edit.component.html',
  styleUrls: ['./quiz-edit.component.css']
})
export class QuizEditComponent implements OnInit {
  title: string;
  quiz: Quiz;

  editMode: boolean;

  constructor(
    private activatedRoute: ActivatedRoute,
    private http: HttpClient,
    private router: Router,
    @Inject('BASE_URL') private baseUrl: string
  ) {
    this.quiz = <Quiz>{};

    var id = +this.activatedRoute.snapshot.params["id"];
    if (id) {
      this.editMode = true;

      var url = this.baseUrl + "api/quiz/" + id;
      this.http.get<Quiz>(url).subscribe(res => {
        this.quiz = res;
        this.title = "Edit - " + this.quiz.Title;
      }, error=>console.error(error));
    }
  }

  onSubmit(quiz: Quiz) {
    var url = this.baseUrl + "api/quiz";
    if (this.editMode) {
      this.http.post<Quiz>(url, quiz)
        .subscribe(res => {
          var v = res;
          console.log(`Quiz ${v.Id} has been updated.`);
          this.router.navigate(['home']);

        }, error => console.error(error));
    } else {
      this.http.put<Quiz>(url, quiz)
        .subscribe(res => {
          var q = res;
          console.log(`Quiz ${q.Id} has been created.`);
          this.router.navigate(['home']);
        }, error => console.error(error));
    }
  }

  onBack() {
    this.router.navigate(['home']);
  }

  ngOnInit() {
  }

}
