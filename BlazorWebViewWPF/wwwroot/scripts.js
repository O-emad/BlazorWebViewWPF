
var videoConstraints = { width: 3840, facingMode: 'environment', frameRate: 30, aspectRatio: (16.0 / 9.0) };
const resolutions = ["3840 x 2160", "3072 x 1728", "2880 x 1620", "2048 x 1152", "1920 x 1080", "1280 x 720"];
var deepAR = null;
var viewCanvas = document.getElementById("deepar-canvas");
viewCanvas.width = 1280;
viewCanvas.height = 720;

var backgroundCanvas = document.createElement("canvas");
backgroundCanvas.setAttribute("id", "background-canvas");

var canvasCopyInterval = setInterval(function () {
}, 16);

var image = new Image();
let isRecording = false;


window.saveFromDataURL = (dataURL) => {
    DotNet.invokeMethodAsync('BlazorWebViewWPF', 'SaveFromDataURL', dataURL)
        .then(data => {
            console.log(data);
        });
};


export function onReload() {
    //re-initialize foregound canvas
    viewCanvas = document.getElementById("deepar-canvas");
    viewCanvas.width = 1280;
    viewCanvas.height = 720;
    //
    //fill the select drop downs items
    for (var i = 0; i < resolutions.length; i++) {

        var option = document.createElement('option')
        var values = resolutions[i].split('x');
        option.value = values[0]
        option.text = resolutions[i];
        document.getElementById('resolutionSelect').appendChild(option);
    }
    for (var i = 30; i <= 90; i = i + 10) {

        var option = document.createElement('option');
        option.value = i;
        option.text = i + " fps";
        document.getElementById('fpsSelect').appendChild(option);
    }
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
    document.getElementById("videoSource").addEventListener("change", function () {
        var d = document.getElementById("videoSource").value;
        if (videoConstraints.deviceId) {
            videoConstraints['deviceId'] = d;
        } else {
            videoConstraints = Object.assign(videoConstraints, { deviceId: d });
        }
        if (deepAR) {
            deepAR.stopVideo();
            deepAR.startVideo(true, videoConstraints);
        }
    });
    document.getElementById("fpsSelect").addEventListener("change", function () {
        var fps = document.getElementById("fpsSelect").value;
        if (deepAR)
            deepAR.setFps(fps);
    });
    document.getElementById("resolutionSelect").addEventListener("change", function () {
        var d = document.getElementById("resolutionSelect").value;
        if (videoConstraints.width) {
            videoConstraints.width = d;
        } else {
            videoConstraints = Object.assign(videoConstraints, { width: d })
        }
        if (deepAR) {
            deepAR.stopVideo();
            deepAR.setCanvasSize(videoConstraints.width, videoConstraints.width / videoConstraints.aspectRatio);
            deepAR.startVideo(true, videoConstraints);
        }
    });
    //
    //reset the copy interval of the background canvas to foreground canvas
    clearInterval(canvasCopyInterval);
    canvasCopyInterval = setInterval(function () {
        if (viewCanvas && backgroundCanvas) {
            var ctx = viewCanvas.getContext('2d');
            ctx.drawImage(backgroundCanvas, 0, 0, 1280, 720);
        }
    }, 16);
    console.log("interval set");
    //
    //clear the background canvas content

    //

    //re-initialize the module
    deepAR = DeepAR({
        licenseKey: 'd6969ce4c33114a74e5e82eaa56aac1f6117b7b2a819a77d5fc69a6d127543d640bf09fa01fd0c92',

        canvas: backgroundCanvas,
        canvasWidth: videoConstraints.width,//getCanvasWidth(),
        canvasHeight: videoConstraints.width / videoConstraints.aspectRatio,//getCanvasHeighWithRatio(getCanvasWidth()),

        numberOfFaces: 4, // how many faces we want to track min 1, max 4
        onInitialize: function () {
            isRecording = false;
            image = new Image();
        },
        onScreenshotTaken: function (photo) {
            window.saveFromDataURL(photo);
            //var link = document.getElementById('link');
            //link.setAttribute('download', 'image.png');
            //link.setAttribute('href', photo.replace('image.png', "image/octet-stream"));
            //link.click();

        },
        onError: function (errorType, message) {
            console.log(errorType + " " + message);
        }
    });


    // download the face tracking model
    deepAR.downloadFaceTrackingModel('models/models-68-extreme.bin');
    console.log("reloaded");
}


export function switchCamera() {
    if (videoConstraints.facingMode == 'environment') {
        videoConstraints.facingMode = 'user';
    } else {
        videoConstraints.facingMode = 'environment';
    }
    if (deepAR) {
        deepAR.stopVideo();
        deepAR.setCanvasSize(videoConstraints.width, videoConstraints.width / videoConstraints.aspectRatio);
        deepAR.startVideo(true, videoConstraints);
    }
}

export function takeScreenshot() {
    deepAR.takeScreenshot();
}
export function setFps(fps) {
    if (deepAR)
        deepAR.setFps(fps);
}

export function videoRecording() {
    if (!isRecording) {
        isRecording = true;
        deepAR.startVideoRecording();
        console.log("Recording started!");
    } else {
        deepAR.finishVideoRecording(function (video) {
            console.log("Recording finished!");
            const reader = new FileReader();
            reader.addEventListener('loadend', () => {
                window.saveFromDataURL(reader.result)
            });
            reader.readAsDataURL(video);
            isRecording = false;
        });
    }
}

export function setResolution(width, height) {
    if (videoConstraints.width) {
        videoConstraints.width = width;
    } else {
        videoConstraints = Object.assign(videoConstraints, { width: width })
    }
    if (videoConstraints.height) {
        videoConstraints.height = height;
    } else {
        videoConstraints = Object.assign(videoConstraints, { height: height })
    }
    if (deepAR) {
        deepAR.stopVideo();
        deepAR.setCanvasSize(videoConstraints.width, videoConstraints.height);
        deepAR.startVideo(true, videoConstraints);
    }
}


export function startVideo(mirror) {
    deepAR.startVideo(mirror, videoConstraints);
}

export function stopVideo() {
    deepAR.stopVideo();
}

export function shutdown() {
    deepAR.shutdown();
    //clearInterval(canvasCopyInterval);
    console.log("disposed");
}

export function switchEffect(path, face, slot) {
    deepAR.switchEffect(face, slot, path, function () {
        console.log("effect changed:" + path)
    })
}

export function clearEffect(slot) {
    deepAR.clearEffect(slot);
}

//window.onresize = setViewCanvasDimensions(0, 0);

function getCanvasWidth() {
    var canvasContainer = document.getElementById('canvas-container');
    var style = getComputedStyle(canvasContainer);
    let paddingLeft = parseInt(style.paddingLeft);
    let paddingRight = parseInt(style.paddingRight);
    let ratio = window.innerHeight / window.innerWidth;
    let scrollbarWidth = (window.innerWidth - document.documentElement.clientWidth) || 0;
    var canvasWidth = canvasContainer.clientWidth - paddingLeft - paddingRight - scrollbarWidth;
    return canvasWidth;
}

//function getCanvasHeighWithRatio(width) {
//    var appbar = document.getElementById('appbar');
//    let ratio = window.innerHeight / window.innerWidth;
//    var canvasHeight = (width * ratio) - appbar.clientHeight * 2 - 10;
//    return canvasHeight;
//}

export function setViewCanvasDimensions(width, height) {
    viewCanvas.width = width || getCanvasWidth();
    viewCanvas.height = height || (viewCanvas.width / videoConstraints.aspectRatio);
}

export function setBackgroundCanvasDimensions(width, height) {
    //var canvasContainer = document.getElementById('canvas-container');
    //var style = getComputedStyle(canvasContainer);
    ////let borderLeftWidth = parseInt(style.borderLeftWidth) || 0;
    ////let borderRightWidth = parseInt(style.borderRightWidth) || 0;
    ////let marginLeft = parseInt(style.marginLeft);
    ////let marginRight = parseInt(style.marginRight);
    //let paddingLeft = parseInt(style.paddingLeft);
    //let paddingRight = parseInt(style.paddingRight);
    //let ratio = window.innerHeight / window.innerWidth;
    //var appbar = document.getElementById('appbar');
    //var canvasWidth = canvasContainer.clientWidth - paddingLeft - paddingRight;

    //var canvasWidth = getCanvasWidth();
    //var canvasHeight = getCanvasHeighWithRatio(canvasWidth);
    deepAR.setCanvasSize(width, height);
    //var canvas = document.getElementById('deepar-canvas');
    //deepAR.canvasWidth = width;
    //deepAR.canvasHeight = height,
    //canvas.width = width;
    //canvas.height = height;
}



export function loadEffect(effect) {
   // deepAR.switchEffect(0, 'makeup', effect, function () {
        // effect loaded, reprocess the image
        deepAR.processImage(image);
   // });
}


export function processPhoto(url) {

    // load image from url
    image.src = url;
    image.onload = function () {

        //deepAR.clearEffect("slot");
        deepAR.processImage(image);

        // Alternative way to process image is by using processFrame method
        // // when loaded, convert to byte array
        //var canvas = document.createElement('canvas');
        //var ctx = canvas.getContext('2d');
        //canvas.width = image.width;
        //canvas.height = image.height;
        //ctx.drawImage(image, 0, 0);
        //var imageData = ctx.getImageData(0, 0, canvas.width, canvas.height);
        //var lastImageByteArray = new Uint8Array(imageData.data.buffer);
        //var lastImageHeight = image.height;
        //var lastImageWidth = image.width;

        ////send the image in RGBA byte array format to DeepAR for processing
        //deepAR.processFrame(lastImageByteArray, image.width, image.height, false);


        // hide the loading animation
    }
}

//video options related functions//

