function load(src)
{
    try {
        if (src) {
            var aud = document.getElementById("audio");
            aud.src = src;
            aud.type = "audio/wav";
            console.log("Loaded Audio");
            aud.type = "audio/wav";
            aud.addEventListener("ended", autoStop);
            return true;
        }
    }
    catch (err) {
        console.log("Error: " + err);
    }
    return false;
}
function getDuration() {
    var aud = document.getElementById("audio");
    return aud.duration;
}
function playpause()
{
    try {
        var aud = document.getElementById("audio");
        if (aud.paused) {
            aud.play();
            console.log("Play Click");
            return true;
        }
        else {
            aud.pause();
            console.log("Pause Click");
            return false;
        }
    }
    catch (err) {
        console.log("Error: " + err);
    }
}

function stop()
{
    try {
        var aud = document.getElementById("audio");
        aud.pause();
        aud.currentTime = 0.0;
        console.log("Stop Click");
    }
    catch (err) {
        console.log("Error: " + err);
    }
    return false;
}

function autoStop()
{
    var checkbox = document.getElementById("AudioFinished");
    checkbox.click();
}
