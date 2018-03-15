import { Component, OnInit,Inject,OnChanges,SimpleChanges } from '@angular/core';
import { Router } from "@angular/router";
import { HttpClient } from "@angular/common/http";
import { Input } from '@angular/core/src/metadata/directives';

@Component({
  selector: 'answer-list',
  templateUrl: './answer-list.component.html',
  styleUrls: ['./answer-list.component.css']
})
export class AnswerListComponent implements OnChanges {
  @Input() question: Question;
  answers: Answer[];
  title: string;
  constructor(
    private http: HttpClient,
    @Inject('BASE_URL') private baseUrl: string,
    private router: Router
  ) {
    this.answers = [];
  }

  ngOnInit() {
  }

  ngOnChanges(changes: SimpleChanges) {
    if (typeof changes['question'] != 'undefined') {
      var change = changes['question'];

      if (!change.isFirstChange()) {
        this.loadData();
      }
    }
  }
  loadData() {
    var url = this.baseUrl + "api/answer/All/" + this.question.Id;
    this.http.get<Answer[]>(url)
      .subscribe(res => {
        this.answers = res;
      }, error => console.error(error));

  }

  onCreate() {
    this.router.navigate(["/answer/create", this.question.Id]);
  }

  onEdit(answer: Answer) {
    this.router.navigate(["/answer/edit", answer.Id]);
  }

  onDelete(answer: Answer) {
    if (confirm("Do you really want to delete this answer?")) {
      var url = this.baseUrl + "api/answer/" + answer.Id;
      this.http.delete(url)
        .subscribe(res => {
          console.log(`Answer ${answer.Id} has beeen deleted.`);

          this.loadData();
        }, error => console.error(error));
    }
  }
}
