
var track = null;
var trackSettings = null;
var video = document.getElementById('video');
var area = document.getElementById('area');
var videoConstraints = { width: 8000, facingMode: 'environment', frameRate: 30, aspectRatio: (16.0 / 9.0) };

if (navigator.mediaDevices && navigator.mediaDevices.getUserMedia) {
    navigator.mediaDevices.enumerateDevices().then(function (devices) {
        devices.forEach(function (device) {
            if (device.kind === 'videoinput') {
                var option = document.createElement('option');
                option.value = device.deviceId;
                option.text = device.label || 'camera ' + (i + 1);
                document.getElementById('videoSource').appendChild(option);
            }
        });
    })
        .catch(function (err) {
            console.log(err.name + ": " + err.message);
        });

}

function startVideo() {
    navigator.mediaDevices.getUserMedia({ video: videoConstraints }).then(function (stream) {
        let videoTracks = stream.getVideoTracks();
        if (videoTracks.length) {
            track = videoTracks[0];
            trackSettings = track.getSettings();
            
        }
        video.srcObject = stream;
        video.play();
        let result = document.getElementById("result");
        result.textContent = "Max Resoultion: " + trackSettings.width + " x " + trackSettings.height
            + ", FrameRate: "+trackSettings.frameRate;
    });
}

document.getElementById('startVideo').addEventListener("click", startVideo);

let result = document.getElementById("result");

if (navigator.mediaDevices.getSupportedConstraints().deviceId) {
    result.textContent = "Supported!";
} else {
    result.textContent = "Not supported!";
}

var canvas = document.getElementById('canvas');
var context = canvas.getContext('2d');
var video = document.getElementById('video');

document.getElementById("snap").addEventListener("click", function () {
    //if (track != null) {
    //    track.applyConstraints({ width: 1920, height: 1080, facingMode: 'user'});
    //}
    if (trackSettings) {
        //canvas.width = trackSettings.width;
        //canvas.height = trackSettings.height;
        //context.scale(trackSettings.width, trackSettings.height);
        var _canvas = document.createElement("canvas");
        _canvas.width = trackSettings.width;
        _canvas.height = trackSettings.height;
        var ctx = _canvas.getContext("2d");
        ctx.drawImage(video, 0, 0);

        canvas.height = canvas.width / (16.0/9.0);
        context.drawImage(video, 0, 0, canvas.width, canvas.height);
        var imageUrl = _canvas.toDataURL("image/png");
        var link = document.getElementById('link');
        link.setAttribute('download', 'image.png');
        link.setAttribute('href',imageUrl);
        link.click();
    }
});


document.getElementById("videoSource").addEventListener("change", function () {
    var d = document.getElementById("videoSource").value;
    if (videoConstraints.deviceId) {
        videoConstraints['deviceId'] = d;
    } else {
        videoConstraints = Object.assign(videoConstraints,{ deviceId: d });
    }
    if (track) {
        track.stop();
        track.applyConstraints({ deviceId: d });
        track = null;
        video.srcObject = null;
        startVideo();
    }
});

document.getElementById("stopVideo").addEventListener("click", function () {
    if (track) {
        track.stop();
    }
    track = null;
    video.srcObject = null;
});