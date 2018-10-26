//function LoadTable(val) {
//    $.ajax ({
//        url: "/Future/Index",
//        contentType: "application/html; charset=utf-8",
//        type: "POST",
//        cache: false,
//        async: true,
//        data: { 'Cumulative' : val },
//        datatype: "html",
//        success: function (result) {
//            $("#refTable").html(result);
//        },
//        error: function (xhr, ajaxOptions, thrownError) {
//            //$("#refTable").html("Post Not Found");
//            alert('An error occurred... Look at the console (F12 or Ctrl+Shift+I, Console tab) for more information!');

//            $('#refTable').html('<p>status code: ' + jqXHR.status + '</p><p>errorThrown: ' + errorThrown + '</p><p>jqXHR.responseText:</p><div>' + jqXHR.responseText + '</div>');
//            console.log('jqXHR:');
//            console.log(jqXHR);
//            console.log('textStatus:');
//            console.log(textStatus);
//            console.log('errorThrown:');
//            console.log(errorThrown);
//        }
//    })
//}

//$(document).ready(function () {
//    $("#EnterCumulative").on("click", function () {
//        var val = $('#Cumulative').val();
//        if (val == "") return;

//        //LoadTable(val);
//    });
//});