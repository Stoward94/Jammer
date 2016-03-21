// By default validator ignores hidden fields.
// change the setting here to ignore nothing
$.validator.setDefaults({ ignore: null });
$.ajaxSetup({ cache: false });

$.validator.setDefaults({
    highlight: function (element) {
        $(element).closest(".form-group").addClass("has-error");
    },
    unhighlight: function (element) {
        $(element).closest(".form-group").removeClass("has-error");
    }
});

//Global opt in for bootstrap popovers
var popOverSettings = {
    trigger: "hover",
    viewport: "#viewport",
    selector: '[data-toggle="popover"]'
}

$('body').popover(popOverSettings);

//Fix to stop notif menu disappering when clicking inside
$('.dropdown-menu').click(function (e) {
    e.stopPropagation();
});

var fetchUserNotifications = function () {

    var replaceTarget = $("#notif-placeholder");

    //If we already have notifications return
    if (replaceTarget.html() !== "") {
        return;
    }

    var loadingSpinner = $("#n-spinner");
    loadingSpinner.show();

    var ajaxOptions = {
        type: "GET",
        url: "/Notifications/GetNotifications"
    }

    $.ajax(ajaxOptions).success(function(data) {
            replaceTarget.replaceWith(data);
            markNotificationAsRead();
        })
        .error(function() {
            replaceTarget.html("<p>Unable to get your notifications</p>");
        })
        .always(function () {
            loadingSpinner.hide();
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


//Add Friend Ajax
$('#add-friend-btn').click(function(e) {
    e.preventDefault();
    var btn = $(this).prop('disabled', true);
    var spinner = btn.children("i").show();

    var form = btn.closest("form");

    $.ajax({
        url: form.attr("action"),
        method: form.attr("method"),
        data: form.serialize
    }).success(function (data) {
        if (data.success) {
            alert("Friend Added");
        }
        else {
            DisplayError(data.responseText);
        }
        btn.hide();

    }).error(function () {
        DisplayError("Error adding friend. Please try again later");
    }).done(function() {
        spinner.hide();
    });
});

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
var tinymceInitialise = function(){
    tinymce.init({
        selector: ".rich-text-area",
        max_width: 700,
        min_height : 150,
        plugins: "emoticons,autolink,link",
        toolbar: "undo redo | bold italic | bullist numlist | link | emoticons",
        menubar: false,
        statusbar: false,
        browser_spellcheck: true,
        default_link_target: "_blank",
        link_title: false,
        relative_urls: false,
        setup: function (ed) {
            ed.on('blur', function (e) {
                tinyMCE.triggerSave();
            });
        }
    });
}

tinymceInitialise();

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
$("input[data-igdb-ac]").autocomplete({
    source: function (request, response) {
    var options = {
        url: "/IGDB/Search",
        dataType: "json",
        data: { q: request.term },
        method: "GET"
    };

    $.ajax(options)
        .done(function (data) {
            response(data);
        });
    },
    minLength: 2,
    select: function (event, ui) {
        $("input[data-igdb-ac]").val(ui.item.label);
        $("#IgdbGameId").val(ui.item.id);
        return false;
    }
})
.autocomplete("instance")._renderItem = function (ul, item) {
    return $("<li>")
      .append("<a>" + item.label + "<br><span class='date-time'>Released: " + item.date + "</span></a>")
      .appendTo(ul);
};

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

//Post comment ajax handler
$('#btn-comment-post').click(function() {
    tinyMCE.triggerSave();
    var comment = $('#comment').val();

    if (comment.length < 1) {
        $('#comment-val-msg').show();
         return;
    }
    $('#comment-val-msg').hide();
    var button = $(this);
    button.prop('disabled', true).html('<i class="fa fa-circle-o-notch fa-pulse"></i>');

    var form = $(this).closest('form');

    $.ajax({
            url: form.attr('action'),
            method: 'POST',
            cache: false,
            data: form.serialize(),
            success: function (data)
            {
                //Insert new comment and clear editor
                $('.comments-feed').append(data);
                tinyMCE.activeEditor.setContent('');
            },
            error: function (xhr, ajaxOptions, thrownError) {
                $('#comment-val-msg').val("Error posting comment. Please try again later").show();
            }
        })
        .always(function() {
            button.prop('disabled', false).html('Post');
        });
});

//Initialise datetime picker
$(".date-picker").datetimepicker({
    format: "DD/MM/YYYY",
    useCurrent: false,
    minDate: new Date().setHours(0, 0, 0, 0)
});

$(".time-picker").datetimepicker({
    format: "HH:mm",
    stepping: 15,
    useCurrent: false
});

//Sessions Search next/previous controls
$("#search-filter-submit").click(function (e) {
    e.preventDefault();

    var form = $('#search-filter');
    $("#Page").val(1);
    var options = {
        url: form.attr("action"),
        method: form.attr("method"),
        data: form.serialize()
    };

    $.ajax(options)
        .done(function (data) {
            $('#sessions-container').html(data);
        });
});

$("body").on("click",".pager .next a", function (e) { SessionSearch(e, $(this)); });
$("body").on("click", ".pager .previous a", function (e) { SessionSearch(e, $(this)); });
$("body").on("click", ".pager .today a", function (e) { SessionSearch(e, $(this)); });

var SessionSearch = function (e, el) {
    e.preventDefault();

    var parent = el.parent();
    parent.addClass("disabled");

    $("#Page").val(el.attr("data-page"));
    var form = $('#search-filter');

    var options = {
        url: el.attr("href"),
        method: "GET",
        data: form.serialize()
    };

    $.ajax(options)
    .done(function (data) {
            $('#sessions-container').html(data);
        })
    .always(function () {
        parent.removeClass("disabled");
        });
}