/*
 * This .js file contains only js that is need for authorised (logged in) users
 */

$(document).ready(function () {

    /**********************************************
                EDIT PROFILE SCRIPTS
    **********************************************/

    $(".file-upload").change(function () {
        var filename = $(this).val().split('\\').pop();
        $(".edit-img-lbl").text(filename);
        $(".file-upload-submit").show();
    });

});