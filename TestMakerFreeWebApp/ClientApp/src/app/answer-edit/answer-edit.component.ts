import { Component, OnInit ,Inject} from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { FormControl, FormBuilder, Validators, FormGroup } from '@angular/forms'
@Component({
  selector: 'answer-edit',
  templateUrl: './answer-edit.component.html',
  styleUrls: ['./answer-edit.component.css']
})
export class AnswerEditComponent implements OnInit {
  title: string;
  answer: Answer;
  editMode: boolean;
  form: FormGroup;

  constructor(private activatedRoute: ActivatedRoute,
    private router: Router,
    private http: HttpClient,
    private fb: FormBuilder,
    @Inject('BASE_URL') private baseUrl: string) {
    this.answer = <Answer>{};
    this.createForm();
    var id = activatedRoute.snapshot.params["id"];

    this.editMode = (this.activatedRoute.snapshot.url[1].path === "edit");

    if (this.editMode) {
      var url = this.baseUrl + "api/answer/" + id;
      this.http.get<Answer>(url)
        .subscribe(res => {
          this.answer = res;
          this.title = "Edit - " + this.answer.Text;
        }, error => console.error(error));

    } else {
      this.answer.QuestionId = id;
      this.title = "Create a new Answer";
    }
  }
  createForm() {
    this.form = this.fb.group({
      Text: ['', Validators.required],
      Value: ['', [Validators.required, Validators.min(-5), Validators.max(5)]]
    })
  }
  updateForm() {
    this.form.setValue({
      Text: this.answer.Text,
      Value: this.answer.Value
    })
  }
  onSubmit() {
    var tempAnswer = <Answer>{};
    tempAnswer.Text = this.form.value.Text;
    tempAnswer.Value = this.form.value.Value;
    tempAnswer.QuestionId = this.answer.QuestionId;

    var url = this.baseUrl + "api/answer";
    if (this.editMode) {
      tempAnswer.Id = this.answer.Id;
      this.http.post<Answer>(url, tempAnswer).
        subscribe(res => {
          var v = res;
          console.log(`Answer ${v.Id} has been updated.`);
          this.router.navigate(['question/edit', v.QuestionId]);
        }, error => console.error(error));
    } else {
      this.http.put<Answer>(url, tempAnswer)
        .subscribe(res => {
          var v = res;
          console.log(`Answer ${v.Id} has been created.`);
          this.router.navigate(['question/edit', v.QuestionId]);
        }, error => console.error(error));
    }
  }

  onBack() {
    this.router.navigate(['question/edit', this.answer.QuestionId]);
  }
  ngOnInit() {
  }
  getFormControl(name: string) {
    return this.form.get(name);
  }

  isValid(name: string) {
    var e = this.getFormControl(name);
    return e && e.valid;
  }
  isChanged(name: string) {
    var e = this.getFormControl(name);
    return e && (e.dirty || e.touched);
  }

  hasError(name: string) {
    var e = this.getFormControl(name);
    return e && (e.dirty || e.touched) && !e.valid;
  }
}
