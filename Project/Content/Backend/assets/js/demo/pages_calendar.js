"use strict";
$(document).ready(function () {
    var b = new Date();
    var e = b.getDate();
    var a = b.getMonth();
    var f = b.getFullYear();
    var c = {};
    if ($("#calendar").width() <= 400) {
        c = { left: "title", center: "", right: "prev,next" }
    } else {
        c = { left: "prev,next", center: "title", right: "month,agendaWeek,agendaDay" }
    }
    $("#calendar").fullCalendar({
        disableDragging: false,
        header: c, editable: true,
        events: []
    })
});