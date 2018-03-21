import { Component, OnInit, Inject } from '@angular/core';
import { FormGroup, FormControl, FormBuilder, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { ValidatorFn } from '@angular/forms/src/directives/validators';

@Component({
  selector: 'register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.less']
})
export class RegisterComponent implements OnInit {
  title: string;
  form: FormGroup;

  constructor(
    private router: Router,
    private fb: FormBuilder,
    private http: HttpClient,
    @Inject('BASE_URL') private baseUrl: string
  ) {
    this.title = "New User Registration";

    this.createForm();
  }

  createForm() {
    this.form = this.fb.group({
      UserName: ['', Validators.required],
      Email: ['',  [
        Validators.required,
        Validators.email
      ]],
      Password: ['', Validators.required],
      PasswordConfirm: ['', Validators.required],
      DisplayName: ['', Validators.required]
    }
      , {
        validator: this.passwordConfirmValidator
      }

    );
  }
  // custom validator to compare 
  //   the Password and passwordConfirm values.
  passwordConfirmValidator(control: FormControl): any {

    // retrieve the two Controls
    let p = control.root.get('Password');
    let pc = control.root.get('PasswordConfirm');

    if (p && pc) {
      if (p.value !== pc.value) {
        pc.setErrors(
          { "PasswordMismatch": true }
        );
      }
      else {
        pc.setErrors(null);
      }
    }
    return null;
  }

  onSubmit() {
    var tempUser = <User>{};
    tempUser.UserName = this.form.value.UserName;
    tempUser.Email = this.form.value.Email;
    tempUser.Password = this.form.value.Password;
    tempUser.DisplayName = this.form.value.DisplayName;

    var url = this.baseUrl + "api/user";

    this.http.put<User>(url, tempUser)
      .subscribe(res => {
        if (res) {
          var v = res;
          console.log(v);
          console.log(`User ${v.UserName} ${v.DisplayName} has been created.`);
          this.router.navigate(["login"]);

        } else {
          this.form.setErrors({
            "register": "User registration failed."
          });
        }
      }, error => console.error(error))
  }

  onBack() {
    this.router.navigate(["home"]);
  }



  //passwordConfirmValidator(group: FormGroup): any {
  //  let p = group.get('Password');
  //  let pc = group.get('PasswordConfirm');
  //  if (p && pc) {
  //    if (p.value !== pc.value) {
  //      pc.setErrors({
  //        "PasswordMismatch": true
  //      });
  //      return {
  //        passwordConfirmValidator: {
  //          valid: false
  //        }
  //      }
  //    } else {
  //      pc.setErrors(null);
  //    }
  //  }
  //  return null;
  //}

  ngOnInit() {
  }

  // retrieve a FormControl
  getFormControl(name: string) {
    return this.form.get(name);
  }
  // returns TRUE if the FormControl is valid
  isValid(name: string) {
    var e = this.getFormControl(name);
    return e && e.valid;
  }
  // returns TRUE if the FormControl has been changed
  isChanged(name: string) {
    var e = this.getFormControl(name);
    return e && (e.dirty || e.touched);
  }
  // returns TRUE if the FormControl is invalid after user changes
  hasError(name: string) {
    var e = this.getFormControl(name);
    return e && (e.dirty || e.touched) && !e.valid;
  }


}
