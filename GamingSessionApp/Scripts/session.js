
//Initialise datetime picker
$("#datepicker").datetimepicker({
    format: "DD/MM/YYYY",
    useCurrent: false,
    minDate: new Date().setHours(0, 0, 0, 0)
});

$("#timepicker").datetimepicker({
    format: "HH:mm",
    stepping: 15,
    useCurrent: false
});