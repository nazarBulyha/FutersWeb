(function ($) {
	"use strict";
	$(".column100").on("mouseover",function(){
		var table1 = $(this).parent().parent().parent();
		var table2 = $(this).parent().parent();
		var verTable = $(table1).data("vertable")+"";
		var column = $(this).data("column") + ""; 

		$(table2).find("."+column).addClass("hov-column-"+ verTable);
		$(table1).find(".row100.head ."+column).addClass("hov-column-head-"+ verTable);
	});

	$(".column100").on("mouseout",function(){
		var table1 = $(this).parent().parent().parent();
		var table2 = $(this).parent().parent();
		var verTable = $(table1).data("vertable")+"";
		var column = $(this).data("column") + ""; 

		$(table2).find("."+column).removeClass("hov-column-"+ verTable);
		$(table1).find(".row100.head ."+column).removeClass("hov-column-head-"+ verTable);
    });

    // this is the id of the form
    $("#idForm").submit(function(e) {
        var form = $(this);
        var url = form.attr("action");

        $.ajax({
            type: "POST",
            url: url,
            async: true,
            data: form.serialize(), // serializes the form's elements.
            success: function(data) {
                $("#refTable").html(data);
            }
        });

        e.preventDefault(); // avoid to execute the actual submit of the form.
    });

    //$("#EnterCumulative").on("click", function () {
    //    var val = $("#cumulativeText").val();
    //    console.log(val);
    //    //if (typeof(val) !== "undefined" && val !== null) {
    //    $.ajax({
    //        url: "/Future/Index",
    //        type: "POST",
    //        cache: false,
    //        contentType: "application/html; charset=utf-8",
    //        //dataType: "text",
    //        async: true,
    //        data: val,
    //        success: function (result) {
    //            $("#refTable").html(result);
    //        }
    //    });

    //    return false;
    //});
})(jQuery);