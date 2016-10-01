var id = uniqueId();

var title = document.getElementsByClassName("watch-title")[0].getAttribute("title");

var uploader = document.getElementsByClassName("yt-user-info")[0].innerText;

var video = document.getElementsByTagName("video")[0];

var timeout = 0;

var repeat = setInterval(function() {
    sendData(title, uploader, video.currentTime, video.duration, video.paused, false);
}, 500);

window.onbeforeunload = function () {
    clearInterval(repeat);
	// send termination flag when tab or browser is closing.
	sendData(title, uploader, video.currentTime, video.duration, video.paused, true);
};

function sendData(title, uploader, currentTime, duration, paused, terminate){
	var xhttp = new XMLHttpRequest();
	xhttp.open("POST", "http://127.0.0.1:60024/", true);
	xhttp.setRequestHeader("Content-Type", "application/json;charset=UTF-8");
	var data = {
		"id" : id,
        "title" : title,
        "uploader": uploader,
		"currentTime": currentTime,
		"duration": duration,
		"paused" : paused,
		"terminate": terminate
    };
	xhttp.send(JSON.stringify(data));
	//return xhttp.status;
}

function uniqueId () {
    // desired length of Id
    var idStrLen = 32;
    // always start with a letter -- base 36 makes for a nice shortcut
    var idStr = (Math.floor((Math.random() * 25)) + 10).toString(36) + "";
    // add a timestamp in milliseconds (base 36 again) as the base
    idStr += (new Date()).getTime().toString(36) + "";
    // similar to above, complete the Id using random, alphanumeric characters
    do {
        idStr += (Math.floor((Math.random() * 35))).toString(36);
    } while (idStr.length < idStrLen);

    return (idStr);
}
