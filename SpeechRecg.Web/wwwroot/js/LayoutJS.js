function selectpage(button_active, button_unactive)
{
    document.getElementById(button_active).style.background = 'white';
    document.getElementById(button_active).style.color = '#253340';

    document.getElementById(button_unactive).style.background = 'transparent';
    document.getElementById(button_unactive).style.color = 'white';
};