
var sessionTimeMill;
var appWarnMinutesMill;
var appWarnMinutes;
var appWarnSeconds
var sessionTime
var keepSessionAliveAction;
var title;

function initPageSession(strTitle, strSessionTime, strAppWarnMinutes, strKeepSessionAliveAction) {
    title = strTitle
    sessionTime = strSessionTime;
    appWarnMinutes = strAppWarnMinutes;
    keepSessionAliveAction = strKeepSessionAliveAction;

    sessionTimeMill = sessionTime * 60 * 1000;
    appWarnMinutesMill = appWarnMinutes * 60 * 1000;

    elapseWarningTime = sessionTimeMill - appWarnMinutesMill;

    setTimeout("sessionWarning()", elapseWarningTime);

}

function sessionWarning() {
    setTimeout("sessionDestroy()", appWarnMinutesMill+2000);
    appWarnSeconds = appWarnMinutes * 60;
    bootbox.dialog({
        message: 'Your session will expire in T - <span class="session-timeout">' + appWarnSeconds + '</span> seconds',
        title: title,
        closeButton:false,
        buttons: {
            success: {
                label: "Keep my session alive",
                className: "btn-success",
                callback: function () {
                    sessionPersist();
                }
            }
        },
        onEscape: function (event, ui) {
            sessionPersist();
        }
    })
    setInterval(countDown, 1000);
}

function countDown() {
    appWarnSeconds--;
    $('.session-timeout').html(appWarnSeconds);
    if (appWarnSeconds == 0) {
        appWarnSeconds = appWarnSeconds+1;
    }
    
}

function sessionDestroy() {
    console.log("log me out");
    document.getElementById('logoutForm').submit();
    //location.reload();
}

function sessionPersist() {
    $.get(keepSessionAliveAction, "_session=ping", function (data, textStatus, jqXHR) { console.log(data) }, "JSON");
    initPageSession(title, sessionTime, appWarnMinutes, keepSessionAliveAction)
}

