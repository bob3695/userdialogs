﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Acr.UserDialogs;
using Xamarin.Forms;


namespace Samples {

    public class MainPage : ContentPage {
        private readonly Label lblResult;
 
        public MainPage() {
			this.lblResult = new Label();
			this.Content = new ScrollView {
				Orientation = ScrollOrientation.Vertical,
				Content = new StackLayout {
	                Children = {
	                    this.lblResult,
	                    Btn("Alert", this.Alert),
	                    Btn("ActionSheet", this.ActionSheet),
	                    Btn("Confirm", this.Confirm),
	                    Btn("Login", this.Login),
						Btn("Manual Loading", this.ManualLoading),
	                    Btn("Prompt", this.Prompt),
						Btn("Prompt /w Text/No Cancel", this.PromptWithTextAndNoCancel),
	                    Btn("Progress", this.Progress),
	                    Btn("Progress (No Cancel)", this.ProgressNoCancel),
	                    Btn("Loading (Black - Default)", () => this.Loading(MaskType.Black)),
                        Btn("Loading (Clear)", () => this.Loading(MaskType.Clear)),
                        Btn("Loading (Gradient iOS)", () => this.Loading(MaskType.Gradient)),
                        Btn("Loading (None)", () => this.Loading(MaskType.Black)),
	                    Btn("Loading (No Cancel)", this.LoadingNoCancel),
                        Btn("Error", this.Error),
                        Btn("Success", this.Success),
						Btn("Toast (Clear - Default)", () => this.Toast(MaskType.Clear)),
						Btn("Toast (Black)", () => this.Toast(MaskType.Black)),
						Btn("Toast (Gradient - iOS)", () => this.Toast(MaskType.Gradient)),
						Btn("Toast (None)", () => this.Toast(MaskType.None)),
                        Btn("Change Default Settings", () => {
                            // CANCEL
                            ActionSheetConfig.DefaultCancelText = ConfirmConfig.DefaultCancelText = LoginConfig.DefaultCancelText = PromptConfig.DefaultCancelText = ProgressDialogConfig.DefaultCancelText = "NO WAY";

                            // OK
                            AlertConfig.DefaultOkText = ConfirmConfig.DefaultOkText = LoginConfig.DefaultOkText = PromptConfig.DefaultOkText = "Sure";

                            // CUSTOM
                            ActionSheetConfig.DefaultDestructiveText = "BOOM!";
                            ConfirmConfig.DefaultYes = "SIGN LIFE AWAY";
                            ConfirmConfig.DefaultNo = "NO WAY";
                            LoginConfig.DefaultTitle = "HIGH SECURITY";
                            LoginConfig.DefaultLoginPlaceholder = "WHO ARE YOU?";
                            LoginConfig.DefaultPasswordPlaceholder = "SUPER SECRET PASSWORD";
                            ProgressDialogConfig.DefaultTitle = "WAIT A MINUTE";

                            UserDialogs.Instance.Alert("Default Settings Updated - Now run samples");
                        }),
                        Btn("Reset Default Settings", () => {
                            // CANCEL
                            ActionSheetConfig.DefaultCancelText = ConfirmConfig.DefaultCancelText = LoginConfig.DefaultCancelText = PromptConfig.DefaultCancelText = ProgressDialogConfig.DefaultCancelText = "Cancel";

                            // OK
                            AlertConfig.DefaultOkText = ConfirmConfig.DefaultOkText = LoginConfig.DefaultOkText = PromptConfig.DefaultOkText = "Ok";

                            // CUSTOM
                            ActionSheetConfig.DefaultDestructiveText = "Remove";
                            ConfirmConfig.DefaultYes = "Yes";
                            ConfirmConfig.DefaultNo = "No";
                            LoginConfig.DefaultTitle = "Login";
                            LoginConfig.DefaultLoginPlaceholder = "User Name";
                            LoginConfig.DefaultPasswordPlaceholder = "Password";
                            ProgressDialogConfig.DefaultTitle = "Loading";

                            UserDialogs.Instance.Alert("Default Settings Restored");
                        })
	                }
				}
            };
        }


        private static Button Btn(string text, Action action) {
            return new Button {
                Text = text,
                Command = new Command(action)
            };
        }


        private async void Alert() {
            await UserDialogs.Instance.AlertAsync("Test alert", "Alert Title");
            this.lblResult.Text = "Returned from alert!";
        }


        private void ActionSheet() {
			var cfg = new ActionSheetConfig()
				.SetTitle("Test Title");

            for (var i = 0; i < 5; i++) {
                var display = (i + 1);
                cfg.Add(
					"Option " + display, 
					() => this.lblResult.Text = String.Format("Option {0} Selected", display)
				);
            }
			cfg.SetDestructive(action: () => this.lblResult.Text = "Destructive BOOM Selected");
			cfg.SetCancel(action: () => this.lblResult.Text = "Cancel Selected");

            UserDialogs.Instance.ActionSheet(cfg);
        }


        private async void Confirm() {
            var r = await UserDialogs.Instance.ConfirmAsync("Pick a choice", "Pick Title");
            var text = (r ? "Yes" : "No");
            this.lblResult.Text = "Confirmation Choice: " + text;
        }


        private async void Login() {
			var r = await UserDialogs.Instance.LoginAsync(new LoginConfig {
				Message = "DANGER"
			});
            this.lblResult.Text = String.Format(
                "Login {0} - User Name: {1} - Password: {2}",
                r.Ok ? "Success" : "Cancelled",
                r.LoginText,
                r.Password
            );
        }


		private void Prompt() {
			UserDialogs.Instance.ActionSheet(new ActionSheetConfig()
				.SetTitle("Choose Type")
				.Add("Default", () => this.PromptCommand(InputType.Default))
				.Add("E-Mail", () => this.PromptCommand(InputType.Email))
                .Add("Name", () => this.PromptCommand(InputType.Name))
				.Add("Number", () => this.PromptCommand(InputType.Number))
				.Add("Password", () => this.PromptCommand(InputType.Password))
                .Add("Numeric Password (PIN)", () => this.PromptCommand(InputType.NumericPassword))
                .Add("Phone", () => this.PromptCommand(InputType.Phone))
                .Add("Url", () => this.PromptCommand(InputType.Url))
			);
		}


		private async void PromptWithTextAndNoCancel() {
			var result = await UserDialogs.Instance.PromptAsync(new PromptConfig {
				Title = "PromptWithTextAndNoCancel",
				Text = "Existing Text",
				IsCancellable = false
			});
			this.lblResult.Text = String.Format("Result - {0}", result.Text);
		}


		private async void PromptCommand(InputType inputType) {
			var msg = String.Format("Enter a {0} value", inputType.ToString().ToUpper());
			var r = await UserDialogs.Instance.PromptAsync(msg, inputType: inputType);
            this.lblResult.Text = r.Ok
                ? "OK " + r.Text
                : "Prompt Cancelled";
        }


        private async void Progress() {
            var cancelled = false;

            using (var dlg = UserDialogs.Instance.Progress("Test Progress")) {
                dlg.SetCancel(() => cancelled = true);
                while (!cancelled && dlg.PercentComplete < 100) {
                    await Task.Delay(TimeSpan.FromMilliseconds(500));
                    dlg.PercentComplete += 2;
                }
            }
            this.lblResult.Text = (cancelled ? "Progress Cancelled" : "Progress Complete");
        }


        private async void ProgressNoCancel() {
            using (var dlg = UserDialogs.Instance.Progress("Progress (No Cancel)")) {
                while (dlg.PercentComplete < 100) {
                    await Task.Delay(TimeSpan.FromSeconds(1));
                    dlg.PercentComplete += 20;
                }
            }
        }


        private void Error() {
            UserDialogs.Instance.ShowError("ERROR!");
        }


        private void Success() {
            UserDialogs.Instance.ShowSuccess("Success");
        }

        private async void Loading(MaskType maskType) {
            var cancelSrc = new CancellationTokenSource();

			using (var dlg = UserDialogs.Instance.Loading("Loading", maskType: maskType)) {
                dlg.SetCancel(cancelSrc.Cancel);

                try {
                    await Task.Delay(TimeSpan.FromSeconds(5), cancelSrc.Token);
                }
                catch { }
            }
            this.lblResult.Text = (cancelSrc.IsCancellationRequested ? "Loading Cancelled" : "Loading Complete");
        }


        private async void LoadingNoCancel() {
            using (UserDialogs.Instance.Loading("Loading (No Cancel)")) 
                await Task.Delay(TimeSpan.FromSeconds(3));

            this.lblResult.Text = "Loading Complete";
        }


		private void Toast(MaskType maskType) {
            this.lblResult.Text = "Toast Shown";
            UserDialogs.Instance.Toast("Test Toast", 3, () => {
                this.lblResult.Text = "Toast Pressed";
            }, maskType);
        }


		private async void ManualLoading() {
			UserDialogs.Instance.ShowLoading("Manual Loading");
			await Task.Delay(3000);
			UserDialogs.Instance.HideLoading();
		}
    }
}
