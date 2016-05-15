
//Click events on session platform
$("a.platform-icon").click(function () {
    //Hide any other active platform
    $("a.platform-icon.selected").each(function () {
        $(this).removeClass("selected");
    });
    //Set this platform active
    $(this).addClass("selected");
    //Set the platform id in hidden field
    $("#PlatformId").val($(this).attr("data-id"));
});

//Post back highlight icon check
if ($("#PlatformId").val() !== "0") {
    var val = $("#PlatformId").val();
    //Highlight the icon
    $("a.platform-icon[data-id='" + val + "']").addClass("selected");
}

//Click events on session type
$("a.type-icon").click(function () {
    //Hide any other active platform
    $("a.type-icon.selected").each(function () {
        $(this).removeClass("selected");
    });
    //Set this platform active
    $(this).addClass("selected");
    //Set the platform id in hidden field
    $("#TypeId").val($(this).attr("data-id"));
});

//Post back highlight icon check
if ($("#TypeId").val() !== "0") {
    var val = $("#TypeId").val();
    //Highlight the icon
    $("a.type-icon[data-id='" + val + "']").addClass("selected");
}


//Calculate End Date time for session
var calculateEndDate = function () {
    var date = $("#ScheduledDate").val();
    var time = $("#ScheduledTime").val();

    var duration = $("#DurationId").val();
    if (duration === "301") {
        $("#end-time").val("Unlimited");
        return;
    }

    var newDate = moment(date + " " + time, "dddd Do MMMM HH:mm");
    newDate.add(duration, "m");
    $("#end-time").val(moment(newDate).format("HH:mm - dddd Do MMMM"));
};
calculateEndDate();

//Bind events to update end date
$("#DurationId").change(calculateEndDate);
$("#ScheduledDate").blur(calculateEndDate);
$("#ScheduledTime").blur(calculateEndDate);


//Goals/Objectives events
$(".goal-item .close").each(function () {
    $(this).click(function () {
        var totalItems = $(".goal-item").length;
        if (totalItems < 3) return false;

        $(this).closest("li").remove();

        //update id's and placeholder for other items
        $(".goal-item").each(function (i) {
            var input = $(this).children("input");
            input.attr("id", "goals[" + (i - 1) + "]");
            input.attr("name", "goals[" + (i - 1) + "]");
            input.attr("placeholder", "Objective / Goal " + i);
        });

    });
});

//Clone template goal and assign index based ID
$("#add-goal").click(function () {

    var totalItems = $(".goal-item").length;
    var index = totalItems - 1;

    var copy = $("#goal-template").clone(true);

    var input = copy.children("input");
    input.attr("id", "goals[" + index + "]");
    input.attr("name", "goals[" + index + "]");
    input.attr("placeholder", "Objective / Goal " + totalItems);

    copy.removeAttr("id");
    copy.removeAttr("hidden");
    copy.appendTo("#goals-ul");
    return false;
});


//Invite Friends Modal, fetch friends ajax
$('#invite-modal').on('show.bs.modal', function (e) {
    
    var options = {
        url: "/Profile/GetUsersFriends",
        method: "GET"
    }

    var updateTarget = $("#invite-users");

    //If we have content return
    if (updateTarget.html().length > 1) return;

    //Show loading spinner
    var spinner = $('#ajaxLoading').show();

    $.ajax(options)
        .success(function (data) {
            spinner.hide();
            updateTarget.replaceWith(data);
        })
        .error(function () {
            updateTarget.text("Oops: Unable to get your friends. Please try again later.");
        });
})


//Create session button
$(".session-create-btn").click(function(e) {
    //e.preventDefault();

    //var dateEl = $("#ScheduledDate");

    //$("#ScheduledDate").val(moment(dateEl.val(), "DD-MMM-YY").unix());

    //$(this).closest("form").submit();
});

$('#ScheduledDatePicker').on("dp.change", function (e) {
    $("#ScheduledDate").val(moment(e.date).format("DD/MM/YYYY"));
});