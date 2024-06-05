# MD.Net

A package containing binaries and a managed wrapper API for minidisc devices.
The binaries are from this project: https://github.com/gavinbenda/platinum-md

A simple example;

```c
var toolManager = new ToolManager();
var formatValidator = new FormatValidator();
var formatManager = new FormatManager(toolManager);
var deviceManager = new DeviceManager(toolManager);
var discManager = new DiscManager(toolManager, formatValidator);

var device = deviceManager.GetDevices().SingleOrDefault();
var currentDisc = discManager.GetDisc(device);
var updatedDisc = currentDisc.Clone();
var title = "MD.Net.Tests - " + Math.Abs(DateTime.Now.Ticks);

updatedDisc.Title = title;
updatedDisc.Tracks.Clear();
foreach (var fileName in new[] { "Track_001.wav", "Track_002.wav", "Track_003.wav" })
{
    var track = updatedDisc.Tracks.Add(fileName, Compression.None);
    track.Name = "MD.Net.Tests - " + updatedDisc.Tracks.Count;
}

var actionBuilder = new ActionBuilder(formatManager);
var actions = actionBuilder.GetActions(device, currentDisc, updatedDisc);

var result = discManager.ApplyActions(device, actions, Status.None, true);
```

Input files must be WAVE, 44.1kHz, 16 bit, stereo. They may be converted depending on the Compression flag (SP, LP2, LP4).
Progress for various operations (Action, Transfer, Encode) are emitted to your IStatus implementation. 
