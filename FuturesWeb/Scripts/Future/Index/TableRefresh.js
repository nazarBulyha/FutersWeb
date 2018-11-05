function LoadTable() {
    $.ajax({
        url: "/Future/Index",
        type: "POST",
        cache: false,
        contentType: "application/html; charset=utf-8",
        async: true,
        data: { data: $("#Cumulative").val() },
        success: function (result) {
            $("#refTable").html(result);
        }
    });

}
$("#EnterCumulative").on("click", function () {
    //var val = $("#Cumulative").val();
    //if (typeof(val) !== "undefined" && val !== null) {
    LoadTable();

    return false;
});