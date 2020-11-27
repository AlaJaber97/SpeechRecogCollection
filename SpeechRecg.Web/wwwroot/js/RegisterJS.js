function selectgender(gender)
{
    if (gender == "male")
    {
        document.getElementById("gril_gender").src = "\\WebImages\\GrilGeneder_unactive.png";
        document.getElementById("young_gender").src = "\\WebImages\\YoungGeneder_active.png";
    }
    else
    {
        document.getElementById("young_gender").src = "\\WebImages\\YoungGeneder_unactive.png";
        document.getElementById("gril_gender").src = "\\WebImages\\GrilGeneder_active.png";
    }
}