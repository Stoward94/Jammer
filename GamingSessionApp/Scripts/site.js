// By default validator ignores hidden fields.
// change the setting here to ignore nothing
$.validator.setDefaults({ ignore: null });

var fetchUserNotifications = function () {

    var replaceTarget = $("#notif-menu");

    //If we already have notifications return
    if (replaceTarget.children().length > 1) return;

    var loadingSpinner = $("#n-spinner");
    loadingSpinner.show();

    var ajaxOptions = {
        type: "GET",
        url: "/Notifications/GetNotifications"
    }

    $.ajax(ajaxOptions).success(function(data) {
            replaceTarget.html(data);
            markNotificationAsRead();
        })
        .error(function() {
            replaceTarget.html("<p>Unable to get your notifications</p>");
        });
};

var markNotificationAsRead = function() {
    //Collect a list of notification ids to send back
    //to the server
    var unreadNotifsIds = [];

    $(".unread").each(function () {
        unreadNotifsIds.push($(this).attr("data-id"));
    });

    //If we have no unread notifications, return.
    if (unreadNotifsIds.length === 0) return;

    var ajaxOptions = {
        type: "POST",
        url: "/Notifications/UpdateNotifications",
        data: { ids: unreadNotifsIds }
    }

    //Make the request
    $.ajax(ajaxOptions).done(function() {
        return;
    });
};

//Notifications dropdown events
$("#notif-toggle").on("show.bs.dropdown", fetchUserNotifications);
$("#notif-toggle").on("hidden.bs.dropdown", function () { $("#notif-count").hide(); });

function addFriendSuccess(result) {
    if (result.success) {
        alert(result.responseText);
    }
}

//Delete Message AJAX
var deleteUserMessage = function (e) {

    e.preventDefault();

    //select element nearest row
    var link = $(this);
    var row = link.closest("tr");

    var ajaxOptions = {
        url: link.attr("href"),
        type: "POST",
        headers: {
            "Content-Type": "application/json",
            "X-HTTP-Method-Override": "DELETE"
        }
    };

    //Make the request
    $.ajax(ajaxOptions).done(function (data) {
        if (data.success) {
            row.remove();
        } else {
            DisplayError(data.responseText);
        }
    });
}

$(".delete-message").each(function () {
    $(this).click(deleteUserMessage);
});

//Display error message
function DisplayError(message) {
    var errorBox = $("#error-box");
    errorBox.html("<strong>Oops!</strong> " + message).removeClass("hidden");

    setTimeout(function() {
        errorBox.hide(400);
    }, 5000);

}

//TinyMCE initialisation
tinymce.init({
    selector: ".rich-text-area",
    max_width : 700,
    plugins: "emoticons,autolink,link",
    toolbar: "undo redo | bold italic | bullist numlist | link | emoticons",
    menubar: false,
    statusbar: false,
    browser_spellcheck: true,
    default_link_target: "_blank",
    link_title: false,
    relative_urls: false
});

//Create message tag/autocomplete
$("#user-autocomplete").tagEditor({
    delimiter: ',',
    placeholder: 'Username',
    maxTags: 10,
    autocomplete: { 'source': "/Profile/GetUsersJson", minLength: 2 },
    onChange: function (field, editor, tags) {
        var targetId = field.attr("data-update-target");
        $(targetId).val(tags);
    }
});

//Mark messages as read
$("#mark-read-btn").click(function () {
    var ids = [];

    var checkedBoxes = $("input[type=checkbox]:checked");

    checkedBoxes.each(function () {
        ids.push($(this).attr("data-id"));
    });

    if (ids.length < 1) {
        DisplayError("No messages selected. Please select a message first");
    }

    var ajaxOptions = {
        url: $(this).attr("data-url"),
        type: "POST",
        data: { ids : ids }
    };

    //Make the request
    $.ajax(ajaxOptions).done(function (data) {
        if (data.success) {
            checkedBoxes.each(function () {
                var icon = $(this).closest('td').next().find('.fa-envelope');
                icon.removeClass('fa-envelope').addClass('fa-envelope-o');
            });
        } else {
            DisplayError(data.responseText);
        }
    });
});

//Games search auto complete
//$("#game-autocomplete").autocomplete({
//    source: function (request, response) {

//        var auth = "&token=K5hOq_p3hvaQ7IyBpw5CD_Z6EJw-rCuiKsYWVwrRS1U";

//        $.ajax({
//            url: "https://www.igdb.com/api/v1/games/search?q=" + request.term + auth,
//            dataType: "jsonp",
//            success: function (data) {
//                response($.map(data, function (value, key) {
//                    return {
//                        label: value,
//                        value: key
//                    };
//                }));
//            }
//        });
//    }
//});

//Session Feedback Tab Ajax
$('a[data-toggle="tab"][data-ajax-url]').on("shown.bs.tab", function(e) {

    //If we have content return
    if ($("#feedbackContainer").html().length > 1) return;

    //Show loading spinner
    var spinner = $('#ajaxLoading').show();
    
    var options = {
        url: $(this).attr("data-ajax-url"),
        method: "GET",
        cache: false
    }

    $.ajax(options)
        .success(function (data) {
            spinner.hide();
            var updateTarget = $('#feedbackContainer');
            updateTarget.html(data);
            //Rating glyphicons initialisation
            $(".rating").each(function () {
                $(this).rating({
                    start: 0,
                    stop: 10,
                    step: 2,
                    filled: 'fa fa-star fa-2x',
                    filledSelected: 'fa fa-star fa-2x',
                    empty: 'fa fa-star-o fa-2x'
                });
            });
        })
        .error(function (data) {

        });
});

//Ajax partial view fetching
var fetchPartial = function(ajaxOptions, onSuccess, onError) {
    $.ajax(ajaxOptions)
        .success(onSuccess(data))
        .error(onError(data));
}

//Rating glyphicons initialisation
$(".rating").each(function () {
    $(this).rating({
        start: 0,
        stop: 10,
        step: 2,
        fractions: 2,
        filled: 'fa fa-star fa-2x',
        filledSelected: 'fa fa-star fa-2x',
        empty: 'fa fa-star-o fa-2x',
        extendSymbol: function (rate) {
            var title;
            $(this).tooltip({
                container: 'body',
                placement: 'bottom',
                trigger: 'manual',
                title: function () {
                    return title;
                }
            });
            $(this).on('rating.rateenter', function (e, rate) {
                title = rate;
                $(this).tooltip('show');
            })
            .on('rating.rateleave', function () {
                $(this).tooltip('hide');
            });
        }
    });
});

$('.rating').on('change', function () {
    var label = $(this).next();
    label.text($(this).val() + " / 10");
});