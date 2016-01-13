namespace PCSX2Bonus {
	using Microsoft.DirectX.DirectInput;
	using System;
	using System.Collections;
	using System.Threading;
	using System.Threading.Tasks;
	using System.Windows;
	using System.Windows.Interop;

	internal sealed class Gamepad {
		private Device _joystick;
		private CancellationTokenSource cts = new CancellationTokenSource();
		public bool IsValid;
		private Task pollingTask;

		public event EventHandler ButtonPressed;

		public event EventHandler DirectionChanged;

		public Gamepad(Window wnd) {
			foreach (DeviceInstance instance in Manager.GetDevices(DeviceClass.GameControl, EnumDevicesFlags.AttachedOnly)) {
				this._joystick = new Device(instance.InstanceGuid);
				break;
			}
			if (this._joystick == null) {
				Tools.ShowMessage("No gamepad found!", MessageType.Error);
			}
			else {
				WindowInteropHelper helper = new WindowInteropHelper(wnd);
				this._joystick.SetCooperativeLevel(helper.Handle, CooperativeLevelFlags.Background | CooperativeLevelFlags.NonExclusive);
				this._joystick.Properties.BufferSize = 0x80;
				this._joystick.Acquire();
				this.IsValid = true;
			}
		}

		private async void BeginPoll() {
			try {
				if (!this.IsValid) {
					return;
				}
				this.IsPolling = true;
			Label_007E:
				if (this.cts.IsCancellationRequested) {
					return;
				}
				this._joystick.Poll();
				BufferedDataCollection bufferedData = this._joystick.GetBufferedData();
				if (bufferedData != null) {
					IEnumerator enumerator = bufferedData.GetEnumerator();
					try {
						while (enumerator.MoveNext()) {
							BufferedData current = (BufferedData)enumerator.Current;
							if (current.ButtonPressedData == 1) {
								int button = Array.FindIndex<byte>(this.Buttons, b => b != 0);
								if (button == -1) {
									continue;
								}
								this.OnButtonPressed(button);
								goto Label_03F5;
							}
							if (current.ButtonPressedData == 0) {
								if (this.LeftStickX > 0xafc8) {
									this.OnDirectionChanged("right");
									await Task.Delay(250);
								}
								else if (this.LeftStickX < 0x4a38) {
									this.OnDirectionChanged("left");
									await Task.Delay(250);
								}
								if (this.LeftStickY > 0xafc8) {
									this.OnDirectionChanged("down");
									await Task.Delay(250);
									continue;
								}
								if (this.LeftStickY < 0x4a38) {
									this.OnDirectionChanged("up");
									await Task.Delay(250);
								}
							}
						}
					}
					finally {
						IDisposable asyncVariable1;
						asyncVariable1 = enumerator as IDisposable;
						if (asyncVariable1 != null) {
							asyncVariable1.Dispose();
						}
					}
				}
			Label_03F5:
				if (this._joystick != null) {
					switch (this.DirectionalButtons[0]) {
						case 0:
							this.OnDirectionChanged("up");
							await Task.Delay(250);
							break;

						case 0x2328:
							this.OnDirectionChanged("right");
							await Task.Delay(250);
							break;

						case 0x4650:
							this.OnDirectionChanged("down");
							await Task.Delay(250);
							break;

						case 0x6978:
							this.OnDirectionChanged("left");
							await Task.Delay(250);
							break;
					}
				}
				goto Label_007E;
			}
			catch (Exception exception) {
				this.IsPolling = false;
				Console.WriteLine(exception.Message);
			}
		}

		public void CancelPollAsync() {
			this.cts.Cancel();
			this.IsPolling = false;
		}

		public void Dispose() {
			if (this._joystick != null) {
				this._joystick.Unacquire();
				this._joystick.Dispose();
				this._joystick = null;
			}
			this.IsValid = false;
		}

		private void OnButtonPressed(int button) {
			if (this.ButtonPressed != null) {
				this.ButtonPressed(button, EventArgs.Empty);
			}
		}

		private void OnDirectionChanged(string direction) {
			if (this.DirectionChanged != null) {
				this.DirectionChanged(direction, EventArgs.Empty);
			}
		}

		public void PollAsync() {
			this.cts = new CancellationTokenSource();
			this.pollingTask = new Task(new System.Action(this.BeginPoll), this.cts.Token);
			this.pollingTask.Start();
		}

		public byte[] Buttons {
			get {
				return this._joystick.CurrentJoystickState.GetButtons();
			}
		}

		public int[] DirectionalButtons {
			get {
				return this._joystick.CurrentJoystickState.GetPointOfView();
			}
		}

		public bool IsPolling { get; set; }

		public int LeftStickX {
			get {
				return this._joystick.CurrentJoystickState.X;
			}
		}

		public int LeftStickY {
			get {
				return this._joystick.CurrentJoystickState.Y;
			}
		}

	}
}

