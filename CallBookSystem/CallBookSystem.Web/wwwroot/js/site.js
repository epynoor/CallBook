// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.



$(document).ready(function () {
    
    $("#ct-chart .ct-series").addClass("pull-up");

    $(".select2").addClass("w-100");

    $("#take-action-check").on('click', function () {
        if (!$("#take-action-check").hasClass("checked")) {
            $("#take-action-check").addClass("checked");

            $("#assign-check").removeClass("checked");
            $("#return-check").removeClass("checked");

            $("#action-collapse").addClass("show");
            $("#assign-collapse").removeClass("show");
            $("#return-collapse").removeClass("show");
        }
        $(".btn-instruction").attr("aria-expanded", "false");
        $(".notes").removeClass("show");
    });
    $("#assign-check").on('click', function (e) {
        if (!$("#assign-check").hasClass("checked")) {
            $("#assign-check").addClass("checked");
        }
       
        $("#take-action-check").removeClass("checked");
        $("#return-check").removeClass("checked");

        $("#action-collapse").removeClass("show");
        $("#assign-collapse").addClass("show");
        $("#return-collapse").removeClass("show");

        $(".btn-instruction").attr("aria-expanded", "false");
        $(".notes").removeClass("show");
    });

    $("#return-check").on('click', function (e) {
        if (!$("#return-check").hasClass("checked")) {
            $("#return-check").addClass("checked");
        }

        $("#take-action-check").removeClass("checked");
        $("#assign-check").removeClass("checked");

        $("#return-collapse").addClass("show");
        $("#action-collapse").removeClass("show");     
        $("#assign-collapse").removeClass("show");

        $(".btn-instruction").attr("aria-expanded", "false");
        $(".notes").removeClass("show");
    });

});

var loaderLoaded = false;

showLoader = function () {
    if (!loaderLoaded) {
        loaderLoaded = true;
        $.blockUI({
            message: '<img src="image/spinner.gif" id="spin-img"/>',
            overlayCSS: {
                backgroundColor: '#b4b4b4',
                opacity: 0.6
            },
            css: {
                border: 0,
                padding: 0,
                color: '#333',
                backgroundColor: 'transparent'
            }
        });
    }
}

hideLoader = function () {
    loaderLoaded = false;
    $.unblockUI();
}
$(window).on('beforeunload', function () {
    showLoader();
});
$(document).on('submit', 'form', function () {
    showLoader();
});


