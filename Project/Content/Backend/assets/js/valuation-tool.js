
   
    $(document).ready(function () {
       
        var cache = {};
        $("#GoodDescription").autocomplete({
            minLength: 2,
            focus: function (event, ui) {
                //$("#GoodDescription").val(ui.item.DescriptionOfGoods);
                //$("#HsCode").val(ui.item.AssessedHSCode);
                return false;
            },
            select: function (event, ui) {
                $("#GoodDescription").val(ui.item.DescriptionOfGoods);
                $("#HsCode").val(ui.item.AssessedHSCode);
                return false;
            },
            source: function (request, response) {
                var term = request.term;
                if (term in cache) {
                    response(cache[term]);
                    return;
                }
                console.log(request.term);
                $.getJSON($('#autocomplete-url').val(), { request: request.term, isGD:true }, function (data, status, xhr) {
                    cache[term] = data;
                    response(data);
                });
            }
        }).autocomplete("instance")._renderItem = function (ul, item) {
            //console.log(item);
            return $("<li>")
            .append("<a>" + item.AssessedHSCode + "<br>" + item.DescriptionOfGoods + "</a>")
            .appendTo(ul);
        };

        $("#HsCode").autocomplete({
            minLength: 1,
            focus: function (event, ui) {
                //$("#GoodDescription").val(ui.item.DescriptionOfGoods);
                //$("#HsCode").val(ui.item.AssessedHSCode);
                return false;
            },
            select: function (event, ui) {
                $("#GoodDescription").val(ui.item.DescriptionOfGoods);
                $("#HsCode").val(ui.item.AssessedHSCode);
                return false;
            },
            source: function (request, response) {
                var term = request.term;
                if (term in cache) {
                    response(cache[term]);
                    return;
                }
                console.log(request.term);
                $.getJSON($('#autocomplete-url').val(), { request: request.term, isGD: false }, function (data, status, xhr) {
                    cache[term] = data;
                    response(data);
                });
            }
        }).autocomplete("instance")._renderItem = function (ul, item) {
            //console.log(item);
            return $("<li>")
            .append("<a>" + item.AssessedHSCode + "<br>" + item.DescriptionOfGoods + "</a>")
            .appendTo(ul);
        };

        $(".datepicker").datepicker({
            changeMonth: true,
            changeYear: true,
            yearRange: '-100:+0',
            dateFormat: "dd-mm-yy"
        });
        dialog = $("#dialog-div").dialog({
            autoOpen: false,
            width: 900,
            modal: true,
            close: function () {

            }
        });
        dialog2 = $("#dialog-details").dialog({
            autoOpen: false,
            height:500,
            width: '98%',
            overflow:"auto",
            modal: true,
            close: function () {

            }
        });
    });


function popupDetails(id) {
    //console.log($(this).attr("id"));
    //$.get("@Url.Action("Details")", { id: $(this).attr("id") }, function (data) { $("#dialog-div").html(data); });
    $("#dialog-details").html($(id).html());
    $("#dialog-details .tbodyrecords").show();
    dialog2.dialog("open");
    return null;
}

function PrintElem(elem) {
    Popup($(elem).html());
}

function Popup(data) {
    var mywindow = window.open('', 'my div', 'height=400,width=600');
    mywindow.document.write('<html><head><title>Product Valuation Tool</title>');
    mywindow.document.write('<link href="/Content/bootstrap/css/bootstrap.min.css" rel="stylesheet"  media="all" /> <link href="/Content/assets/css/plugins/jquery-ui.css" rel="stylesheet" media="all" /> <link href="/Content/assets/css/main.css" rel="stylesheet" media="all" /> <link href="/Content/assets/css/plugins.css" rel="stylesheet" media="all" /> <link href="/Content/assets/css/icons.css" rel="stylesheet" media="all" /> <link href="/Content/assets/css/fontawesome/font-awesome.min.css" rel="stylesheet" media="all" />');
    mywindow.document.write('</head><body>');
    mywindow.document.write(data);
    mywindow.document.write('</body></html>');
    mywindow.print();
    //mywindow.close();
    return true;
}
function ExportToExcel(url) {
    var urlbuffer = $("#formid").attr("action");
    $("#formid").attr("action",url);
    //console.log($("#formid").action);
    //alert();
    $("#formid").submit();
    $("#formid").attr("action", urlbuffer);

}
function getPage(e){
    var page = $(e).attr("id");
    $("#page").val(page);
    $("#formid").submit();
}

function showItemDetails(id, rarnumber, url, idtag) {
    if (idtag == '') {
        $('#item-' + id).toggle();
        $.get(url, { RARNumber: rarnumber, id: id, '_': $.now() },
            function (data) { $('#item-' + id).html(data); });
    } else {
        $(idtag + ' #item-' + id).toggle();
        $.get(url, { RARNumber: rarnumber, id: id, '_': $.now() },
            function (data) { $(idtag + ' #item-' + id).html(data); });
    }
}

function recalculate(idtag) {
    var sum = 0;
    var cnt = 0;
    var avg = 0;
    var min = 0;
    var max = 0;
    var per = 0;
    var fob = [];
    var indicator = "";
    var declareUnitFOB = parseFloat($("#DeclaredUnitFOB").val())
    var thrh = parseFloat($("#threshold").val());
    /////////////Hide All///////////
        
    $(idtag + " input[type=checkbox]:checked.items").each(function () {
        console.log($(this).attr("id"));
        sum += parseFloat($(this).attr("id"));
        console.log("sum: " + sum);
        fob[cnt] = parseFloat($(this).attr("id"));
        ++cnt;
        //  $(this).closest(".tr").removeClass("hide");
    });
    if (cnt == 0) { alert("Please select atleast one record."); return null; }
    $(idtag + " tbody input[type=checkbox].items").each(function () {
        $(this).closest(".tr").addClass("hide");
    });
    $(idtag + " input[type=checkbox]:checked.items").each(function () {
        $(this).closest(".tr").removeClass("hide");
    });
    //cnt--;
    console.log("fob: " + fob);
    console.log("sum: "+sum);
    console.log("cnt: "+cnt);
    avg = sum / cnt;
    console.log("avg: " + avg);
    console.log("thrh: " + thrh);
    per = (avg * thrh);
    console.log("per: " + per);
    min = Math.min.apply(Math, fob); // 3
    console.log("min: " + min);
    max = Math.max.apply(Math, fob); // 100
    console.log("max: " + max);
    $(idtag + " td span#avg").html(avg.toFixed(2));
    $(idtag + " td span#min").html(min.toFixed(2));
    $(idtag + " td span#max").html(max.toFixed(2));
    $(idtag + " td div#cnt").html(cnt);
    if (declareUnitFOB < (avg-(avg*thrh))) {
        indicator = "#CC3300";
    } else if (declareUnitFOB > (avg + (avg * thrh))) {
        indicator = "#c7cc06";
    } else if (declareUnitFOB >= (avg - (avg * thrh)) && declareUnitFOB <= (avg + (avg * thrh))) {
        indicator = "#537e32";
    }
    if (indicator != null) {
        $(idtag + " td#indicator").attr("style", "background-color:" + indicator + ";");
    }
}
   

function ResetFormFields(formId) {
    $(formId + ' *').filter(':input[type="text"]').each(function () {
        $(this).val('');
       //alert(this);
    });
    $(formId + ' select').each(function () {
        $(this).val('');
        //alert(this);
    });
    $("#Quantity").val('0')
    $("#DeclaredUnitFOB").val('0')
    $("#MinAssessedUnitFOB").val('0')
    $("#MaxAssessedUnitFOB").val('0')

    //$(formId + ' *').filter(':select').each(function () {
    //    $(this).val('');
    //    //alert(this);
    //});
}