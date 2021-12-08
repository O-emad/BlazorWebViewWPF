

var attempts = 0;
var connectionStatus = true;

function connectionCheck() {
    attempts++;
    console.log("attempt: " + attempts);
    if (attempts >= 67) {
        connectionStatus = false;
        
    }
}

let handler;

window.Connection = {
    Initialize: function (interop) {

        handler = function () {
            interop.invokeMethodAsync("Connection.StatusChanged", navigator.onLine);
        }

        window.addEventListener("online", handler);
        window.addEventListener("offline", handler);

        handler(navigator.onLine);
    },
    Dispose: function () {

        if (handler != null) {

            window.removeEventListener("online", handler);
            window.removeEventListener("offline", handler);
        }
    }
};