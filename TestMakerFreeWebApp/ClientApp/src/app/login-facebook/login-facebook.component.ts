import { Component, OnInit, Inject,NgZone, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import {HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { AuthService} from '../services/auth.service';
import { TokenResponse } from '../services/token.response';


declare var window: any;
declare var FB: any;

@Component({
  selector: 'login-facebook',
  templateUrl: './login-facebook.component.html',
  styleUrls: ['./login-facebook.component.less']
})
export class LoginFacebookComponent implements OnInit {

  constructor(
    private http: HttpClient,
    private router: Router,
    private authService: AuthService,
    private zone: NgZone,
    @Inject(PLATFORM_ID) private platformId: any,
    @Inject('BASE_URL') private baseUrl: string
  ) { }

  ngOnInit() {
    if (!isPlatformBrowser(this.platformId)) {
      return;
    }

    if (typeof (FB) === 'undefined') {
      console.log("initialize FB");
      window.fbAsyncInit = () => {
        this.zone.run(() => {
          FB.init({
            appId: '___YOUR-APP-ID___',
            xfbml: true,
            version: 'v2.10'
          });
          FB.AppEvents.logPageView();

          FB.Event.subscribe('auth.statusChange', (
            (result: any) => {
              console.log("FB auth status changed");
              console.log(result);
              if (result.status === 'connected') {
                console.log('Connected to Facebook');
                this.onConnect(result.authResponse.accessToken);
              }
            }
          ),error=>console.error(error))
        });
        (function (d, s, id) {
          var js, fjs = d.getElementsByTagName(s)[0];
          if (d.getElementById(id)) {
            return;
          }
          js = d.createElement(s);
          js.id = id;
          (<any>s).src = "//connect_facebook.net/en_US/sdk.js";
          fjs.parentNode!.insertBefore(js, fjs);
        }(document, 'script', 'facebook-jssdk'));
      }
    } else {
      window.FB.XFMBL.parse();
      FB.getLoginStatus(function (response: any) {
        if (response.status === 'connected') {
          FB.logout(function (res: any) {
            // do nothing
          });
        }
      });

    }
  }

  onConnect(accessToken: string) {
    var url = this.baseUrl + "api/token/facebook";
    var data = {
      access_token: accessToken,
      client_id: this.authService.clientId
    };

    this.http.post<TokenResponse>(url, data)
      .subscribe(res => {
        if (res) {
          console.log('Login successful.');
          console.log(res);
          this.authService.setAuth(res);
          this.router.navigate(['home']);
        } else {
          console.log('Authentication failed');
        }
      }, error => console.error(error));
  }

}
