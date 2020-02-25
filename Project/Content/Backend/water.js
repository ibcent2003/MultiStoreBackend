function GetUOM(url) {
    var id = $("#ProductId").val();
    $.ajax({
        url: url,
        dataType: 'json',
        type: 'GET',
        data: { Id: id },
        cache: false,
        success: function (data) {
            $("#uom").html(data.UOM);
            $("#up").html(data.UnitPrice);
            $("#UnitPrice").val(data.UnitPrice);
        }
    });
}


function GetMarketValue1(url) {
    var id = $("#ProductId").val();
    var quantity = $("#Quantity").val();
    $.ajax({
        url: url,
        dataType: 'json',
        type: 'GET',
        data: { Id: id, Quantity: quantity },
        cache: false,
        success: function (data) {
            $("#market_value").val(data.MarketValue);
        }
    });
}

function GetMarketValue(url, productid, quantity, divid) {

    $.ajax({
        url: url,
        dataType: 'json',
        type: 'GET',
        data: { Id: productid, Quantity: quantity },
        cache: false,
        success: function (data) {
            $("#" + divid).html(data.MarketValue);
            var total = 0;
            $(".mktvalue").each(function (index, value) {
                var strvalue = $(this).html();
                if (strvalue != '') {
                    var arrvalue = strvalue.split('.');
                    var strvalue = arrvalue[0];
                    var strvalue = strvalue.replace(',', '');
                    var intvalue = parseInt(strvalue);
                    console.log('div' + index + ':' + intvalue);
                    total += intvalue;
                }

            });
            $("#total_market_value").html((total + "").replace(/(\d)(?=(\d{3})+(?!\d))/g, "$1,"));
        }
    });
}