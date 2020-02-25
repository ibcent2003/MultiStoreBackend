
    function RefreshDatepicker() {
        $(".datepicker").datepicker({
            defaultDate: +7,
            showOtherMonths: true,
            autoSize: true,
            //
            //dateFormat: "dd-mm-yy"
            dateFormat: "dd-mm-yy"
        });
    }
function CallMethodAction(i, divid) {
    var url = $(i.form).attr("action");
    //console.log(url+divid);
    $.ajax({
        url: url,
        type: 'POST',
        cache: false,
        data: $(i.form).serialize(),
        start: NProgress.start(),
        stop: NProgress.done(),
        success: function (msg) {
            //$.noty.closeAll();
            //var n = noty({ text: 'Details have been updated successfully ', layout: 'bottom', type: 'success', dismissQueue: true, killer: true });
            //setTimeout(n.close(), 500);
            $('#' + divid).html(msg);
            RefreshDatepicker()
        },
        error: function (xhr, status, error) {
            //alert(xhr.responseText);
            //$.noty.closeAll();
            //var n = noty({ text: 'Please check the details submitted for error', layout: 'bottom', type: 'information', dismissQueue: true, killer: true });
            //$('#' + divid).html(xhr.responseText);
            RefreshDatepicker()
        }
    });
}

 function DeriveFromTotalCIF(i, divid) {
        var url = $(i.form).attr("action");
        console.log(url+divid);
        $.ajax({
            url: url,
            type: 'POST',
            cache: false,
            data: $(i.form).serialize(),
            start: NProgress.start(),
            stop: NProgress.done(),
            success: function (msg) {
                //$.noty.closeAll();
                //var n = noty({ text: 'Details have been updated successfully ', layout: 'bottom', type: 'success', dismissQueue: true, killer: true });
                //setTimeout(n.close(), 500);
                $('#' + divid).html(msg);
                RefreshDatepicker()
            },
            error: function (xhr, status, error) {
                //alert(xhr.responseText);
                //$.noty.closeAll();
                //var n = noty({ text: 'Please check the details submitted for error', layout: 'bottom', type: 'information', dismissQueue: true, killer: true });
                $('#' + divid).html(xhr.responseText);
                //RefreshDatepicker()
            }
        });
    }

 $(document).ready(function(){
     setInterval($.noty.closeAll(), 1000);
 })
