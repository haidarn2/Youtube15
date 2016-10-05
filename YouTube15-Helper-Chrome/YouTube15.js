var id;
var repeat;
var title;
var uploader;
var video;

// event listeners for Youtube javascript page traversal.
document.addEventListener("spfdone", process);
document.addEventListener("DOMContentLoaded", process);

function process(){
	stop();
	start();
}
	
// Detect tab/window close
window.onbeforeunload = function () {
	stop();
};


function start(){
	title = document.getElementsByClassName("watch-title")[0].getAttribute("title");
	uploader = document.getElementsByClassName("yt-user-info")[0].innerText;
	video = document.getElementsByTagName("video")[0];
	if (title && uploader && video){		
		id = uniqueId();
		repeat = setInterval(function() {
		sendData(title, uploader, video.currentTime, video.duration, video.paused, false);
	}, 500);
	}
}

function stop(){
	if (repeat != null){
		clearInterval(repeat);
		sendData(title, uploader, video.currentTime, video.duration, video.paused, true);
	}
	repeat = null;
}

function sendData(title, uploader, currentTime, duration, paused, terminate){
	var xhttp = new XMLHttpRequest();
	xhttp.open("POST", "http://127.0.0.1:60024/", true);
	//xhttp.setRequestHeader("Content-Type", "application/json;charset=UTF-8");
	var data = {
		"id" : id,
        "title" : title,
        "uploader": uploader,
		"currentTime": currentTime,
		"duration": duration,
		"paused" : paused,
		"terminate": terminate
    };
	//console.log(id + " " + title + " " + terminate);
	//console.log(JSON.stringify(data));
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
