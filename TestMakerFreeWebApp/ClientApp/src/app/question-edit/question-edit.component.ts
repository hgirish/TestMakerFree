import { Component, Inject, OnInit } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { HttpClient } from "@angular/common/http";
import { FormGroup, FormBuilder, Validators, FormControl } from '@angular/forms'
import { Question } from "../interfaces/question";
@Component({
  selector: 'question-edit',
  templateUrl: './question-edit.component.html',
  styleUrls: ['./question-edit.component.css']
})
export class QuestionEditComponent implements OnInit {
  title: string;
  question: Question;
  editMode: boolean;
  form: FormGroup;
  activityLog: string;

  constructor(private activatedRoute: ActivatedRoute,
    private router: Router,
    private http: HttpClient,
    private fb: FormBuilder,
    @Inject('BASE_URL') private baseUrl: string) {
    this.question = <Question>{};
    this.createForm();

    var id = +this.activatedRoute.snapshot.params["id"];

    this.editMode = (this.activatedRoute.snapshot.url[1].path === "edit");

    if (this.editMode) {
      var url = this.baseUrl + "api/question/" + id;
      this.http.get<Question>(url)
        .subscribe(res => {
          this.question = res;
          this.title = "Edit - " + this.question.Text;
          this.updateForm();
        },error=>console.error(error));
    } else {
      this.question.QuizId = id;
      this.title = 'Create a new Question';
    }
  }

  createForm() {
    this.form = this.fb.group({
      Text: ['', Validators.required]
    });
    this.activityLog = '';
    this.log("Form has been initialized.");

    this.form.valueChanges
      .subscribe(val => {
        if (!this.form.dirty) {
          this.log("Form Model has been loaded.")
        } else {
          this.log("Form was updated by the user.")
        }
      });
    this.form.get('Text')!.valueChanges
      .subscribe(val => {
        if (!this.form.dirty) {
          this.log("Text control has been loaded with initial values.");
        } else {
          this.log("Text control was updated by the user.")
        }
      })
  }

  log(str: string) {
    this.activityLog += "[" + new Date().toLocaleString()
      + "] " + str + "<br />";
  }

  updateForm() {
    this.form.setValue({
      Text : this.question.Text
    });
  }

  onSubmit() {
    var tempQuestion = <Question>{};
    tempQuestion.Text = this.form.value.Text;
    tempQuestion.QuizId = this.question.QuizId;

    console.log(`question: ${tempQuestion.QuizId} ${tempQuestion.Text}`);
    var url = this.baseUrl + "api/question";
    if (this.editMode) {
      tempQuestion.Id = this.question.Id;

      this.http.post<Question>(url, tempQuestion)
        .subscribe(res => {
          var v = res;
          console.log(`Question ${v.Id} has been updated.`);
          this.router.navigate(["quiz/edit", v.QuizId]);
        }, error => console.log(error));
    } else {
      this.http.put<Question>(url, tempQuestion)
        .subscribe(res => {
          var v = res;
          console.log(`Question ${v.Id} has been created.`);
          this.router.navigate(['quiz/edit', v.QuizId]);
        }, error => console.error(error));
    }
  }

  onBack() {
    this.router.navigate(["quiz/edit", this.question.QuizId]);
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
