using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using Microsoft.DirectX.DirectInput;

namespace PCSX2Bonus.Legacy {
	internal sealed class Gamepad {
		private Device _joystick;
		private CancellationTokenSource cts = new CancellationTokenSource();
		public bool IsValid;
		private Task _pollingTask;

		public event EventHandler ButtonPressed;

		public event EventHandler DirectionChanged;

		public Gamepad(Window wnd) {
			foreach (DeviceInstance instance in Manager.GetDevices(DeviceClass.GameControl, EnumDevicesFlags.AttachedOnly)) {
				_joystick = new Device(instance.InstanceGuid);
				break;
			}
			if (_joystick == null) {
				Tools.ShowMessage("No gamepad found!", MessageType.Error);
			}
			else {
				var helper = new WindowInteropHelper(wnd);
				_joystick.SetCooperativeLevel(helper.Handle, CooperativeLevelFlags.Background | CooperativeLevelFlags.NonExclusive);
				_joystick.Properties.BufferSize = 0x80;
				_joystick.Acquire();
				IsValid = true;
			}
		}

		private async void BeginPoll() {
			try {
				if (!IsValid) {
					return;
				}
				IsPolling = true;
			Label_007E:
				if (cts.IsCancellationRequested) {
					return;
				}
				_joystick.Poll();
				var bufferedData = _joystick.GetBufferedData();
				if (bufferedData != null) {
					var enumerator = bufferedData.GetEnumerator();
					try {
						while (enumerator.MoveNext()) {
							var current = (BufferedData)enumerator.Current;
							if (current.ButtonPressedData == 1) {
								var button = Array.FindIndex(Buttons, b => b != 0);
								if (button == -1) {
									continue;
								}
								OnButtonPressed(button);
								goto Label_03F5;
							}
							if (current.ButtonPressedData != 0) continue;
							if (LeftStickX > 0xafc8) {
								OnDirectionChanged("right");
								await Task.Delay(250);
							}
							else if (LeftStickX < 0x4a38) {
								OnDirectionChanged("left");
								await Task.Delay(250);
							}
							if (LeftStickY > 0xafc8) {
								OnDirectionChanged("down");
								await Task.Delay(250);
								continue;
							}
							if (LeftStickY < 0x4a38) {
								OnDirectionChanged("up");
								await Task.Delay(250);
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
				if (_joystick != null) {
					switch (DirectionalButtons[0]) {
						case 0:
							OnDirectionChanged("up");
							await Task.Delay(250);
							break;

						case 0x2328:
							OnDirectionChanged("right");
							await Task.Delay(250);
							break;

						case 0x4650:
							OnDirectionChanged("down");
							await Task.Delay(250);
							break;

						case 0x6978:
							OnDirectionChanged("left");
							await Task.Delay(250);
							break;
					}
				}
				goto Label_007E;
			}
			catch (Exception exception) {
				IsPolling = false;
				Console.WriteLine(exception.Message);
			}
		}

		public void CancelPollAsync() {
			cts.Cancel();
			IsPolling = false;
		}

		public void Dispose() {
			if (_joystick != null) {
				_joystick.Unacquire();
				_joystick.Dispose();
				_joystick = null;
			}
			IsValid = false;
		}

		private void OnButtonPressed(int button) {
			if (ButtonPressed != null) {
				ButtonPressed(button, EventArgs.Empty);
			}
		}

		private void OnDirectionChanged(string direction) {
			if (DirectionChanged != null) {
				DirectionChanged(direction, EventArgs.Empty);
			}
		}

		public void PollAsync() {
			cts = new CancellationTokenSource();
			_pollingTask = new Task(BeginPoll, cts.Token);
			_pollingTask.Start();
		}

		public byte[] Buttons {
			get {
				return _joystick.CurrentJoystickState.GetButtons();
			}
		}

		public int[] DirectionalButtons {
			get {
				return _joystick.CurrentJoystickState.GetPointOfView();
			}
		}

		public bool IsPolling { get; set; }

		public int LeftStickX {
			get {
				return _joystick.CurrentJoystickState.X;
			}
		}

		public int LeftStickY {
			get {
				return _joystick.CurrentJoystickState.Y;
			}
		}

	}
}

