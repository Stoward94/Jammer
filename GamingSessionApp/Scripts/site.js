
var fetchUserNotifications = function() {

    var replaceTarget = $('#notif-menu');

    //If we already have notifications return
    if (replaceTarget.children().length > 1) return;

    var loadingSpinner = $('#n-spinner');
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

    $('.unread').each(function () {
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
$('#notif-toggle').on('show.bs.dropdown', fetchUserNotifications);
$('#notif-toggle').on('hidden.bs.dropdown', function () { $('#notif-count').hide(); });

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
    var row = link.closest('tr');

    var ajaxOptions = {
        url: link.attr('href'),
        type: 'POST',
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

$('.delete-message').each(function () {
    $(this).click(deleteUserMessage);
});

//Display error message
function DisplayError(message) {
    var errorBox = $('#error-box');
    errorBox.html('<strong>Oops!</strong> ' + message).removeClass('hidden');

    setTimeout(function() {
        errorBox.hide(400);
    }, 5000);

}