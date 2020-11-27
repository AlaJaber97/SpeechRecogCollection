URL = window.URL || window.webkitURL;

var gumStream; 						//stream from getUserMedia()
var recorder; 						//MediaRecorder object
var chunks = [];					//Array of chunks of audio data from the browser
var extension;
var url_file;
// true on chrome, false on firefox
console.log("audio/webm:" + MediaRecorder.isTypeSupported('audio/webm;codecs=opus'));
// false on chrome, true on firefox
console.log("audio/ogg:" + MediaRecorder.isTypeSupported('audio/ogg;codecs=opus'));

if (MediaRecorder.isTypeSupported('audio/webm;codecs=opus')) {
	extension = "webm";
} else {
	extension = "ogg"
}

function startRecording() {
	console.log("recordButton clicked");

	/*
		Simple constraints object, for more advanced audio features see
		https://addpipe.com/blog/audio-constraints-getusermedia/
	*/

	var constraints = { audio: true }

	/*
    	We're using the standard promise based getUserMedia() 
    	https://developer.mozilla.org/en-US/docs/Web/API/MediaDevices/getUserMedia
	*/

	navigator.mediaDevices.getUserMedia(constraints).then(function (stream) {
		console.log("getUserMedia() success, stream created, initializing MediaRecorder");

		/*  assign to gumStream for later use  */
		gumStream = stream;

		var options = {
			audioBitsPerSecond: 256000,
			videoBitsPerSecond: 2500000,
			bitsPerSecond: 2628000,
			mimeType: 'audio/' + extension + ';codecs=opus'
		}
		/* 
			Create the MediaRecorder object
		*/
		recorder = new MediaRecorder(stream, { mimeType: 'audio/' + extension + ';codecs=opus'});

		//when data becomes available add it to our attay of audio data
		recorder.ondataavailable = function (e) {
			console.log("recorder.ondataavailable:" + e.data);

			console.log("recorder.audioBitsPerSecond:" + recorder.audioBitsPerSecond)
			console.log("recorder.videoBitsPerSecond:" + recorder.videoBitsPerSecond)
			console.log("recorder.bitsPerSecond:" + recorder.bitsPerSecond)
			// add stream data to chunks
			chunks.push(e.data);
			// if recorder is 'inactive' then recording has finished
			//if (recorder.state == 'inactive') {
			//	// convert stream data chunks to a 'webm' audio format as a blob
			//	const blob = new Blob(chunks, { type: 'audio/' + extension, bitsPerSecond: 128000 });
			//	url_file = createDownloadLink(blob)
			//}
		};

		recorder.onerror = function (e) {
			console.log(e.error);
		}

		//start recording using 1 second chunks
		//Chrome and Firefox will record one long chunk if you do not specify the chunck length
		recorder.start(1000);

		//recorder.start();
	}).catch(function (err) {
		console.log(err);
	});
}

function pauseRecording() {
	console.log("pauseButton clicked recorder.state=", recorder.state);
	if (recorder.state == "recording") {
		//pause
		recorder.pause();
	} else if (recorder.state == "paused") {
		//resume
		recorder.resume();
	}
}

async function stopRecording() {
	try {
		console.log("stopButton clicked");

		//tell the recorder to stop the recording
		recorder.stop();
		//stop microphone access
		gumStream.getAudioTracks()[0].stop();

		const blob = new Blob(chunks, { type: 'audio/' + extension, bitsPerSecond: 128000 });
		url_file = URL.createObjectURL(blob);
		//createDownloadLink(blob);

		var reader = new FileReader();
		reader.readAsDataURL(blob);
		var file = await readUploadedFileAsText(blob)
		var obj = { 'path': url_file, 'data': file }
		return JSON.stringify(obj);
	}
	catch (err) {
		console.log("Error: " + err);
	}
}
const readUploadedFileAsText = (inputFile) => {
	const temporaryFileReader = new FileReader();

	return new Promise((resolve, reject) => {
		temporaryFileReader.onerror = () => {
			temporaryFileReader.abort();
			reject(new DOMException("Problem parsing input file."));
		};

		temporaryFileReader.onload = () => {
			resolve(temporaryFileReader.result);
		};
		//temporaryFileReader.readAsText(inputFile);
		temporaryFileReader.readAsDataURL(inputFile);
	});
};

function createDownloadLink(blob) {
	var url = URL.createObjectURL(blob);
	var link = document.createElement('a');
	//link the a element to the blob 
	link.href = url;
	link.download = 'SpeechRecog.wav';
	link.innerHTML = link.download;
	link.click();
}

//**dataURL to blob**
function dataURLtoBlob(dataurl) {
	var arr = dataurl.split(','), mime = arr[0].match(/:(.*?);/)[1],
		bstr = atob(arr[1]), n = bstr.length, u8arr = new Uint8Array(n);
	while (n--) {
		u8arr[n] = bstr.charCodeAt(n);
	}
	return new Blob([u8arr], { type: mime });
}

//**blob to dataURL**
function blobToDataURL(blob, callback) {
	var a = new FileReader(); 
	a.onload = function (e) { callback(e.target.result); }
	a.readAsDataURL(blob);
}