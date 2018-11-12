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

    $(document).ready(function() {
        function reloadTableAsync() {
            $.ajax({
                type: "POST",
                url: "/Future/Index",
                async: true,
                data: {
                    number: $("#cumulativeText").val()
                },
                success: function(result) {
                    if (result !== null && result !== "") {
                        $("#refTable").html(result);
                        reloadTableAsync();
                    } else {
                        alert("Invalid data!");
                    }
                },
                error: function() {
                    console.log("An error occurred.");
                }
            });
        }

        reloadTableAsync();

        $("#cumulativeText").on("keypress", function (e) {
            if (e.keyCode === 13 || e.which === 13) {
                reloadTableAsync();
            }
        });
    });
})(jQuery);