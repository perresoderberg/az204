

var updategeneratelist = (typeOfList, theList) => {
    if (typeOfList == "easy")
        document.querySelector('#easy').innerHTML = theList;
    if (typeOfList == "hard")
        document.querySelector('#hard').innerHTML = theList;

};
var reset = () => {
    console.log("Enter reset");

    hideexplantion();

    var select = document.querySelectorAll('select[name="selectoption"]');
    select.forEach(s => {
        s.value = "";
    });

    document.querySelectorAll('#div_answers div').forEach(x =>
    {
        console.log("    " + x.id + "  ");
        x.style.backgroundColor = 'white';
    });
    console.log("Exit reset");
};

var showexplantion = () => {
    console.log("  Enter showexplantion");

    if (document == null) {
        console.log("    document == null - Return");
        return;
    }
    if (document.querySelector('#explanation') == null) {
        console.log("    explanation == null - Return");
        return;
    }

    document.querySelector('#explanation').style.visibility = 'visible';

    console.log("  Exit showexplantion");
}
var hideexplantion = () =>
{
    console.log("  Enter hideexplantion");

    if (document == null)
    {
        console.log("    document == null - Return");
        return;
    }
    if (document.querySelector('#explanation') == null) {
        console.log("    explanation == null - Return");
        return;
    }

    document.querySelector('#explanation').style.visibility = 'hidden';

    console.log("  Exit hideexplantion");
}

var checkanswer = (div_my_selection, answer_kind, correct_select_answers, correct_dropdown_answers) => {
    console.log("************************************************************");
    console.log("Enter checkanswer");
    if (correct_dropdown_answers != null)
    {
        console.log("  Loop correct_dropdown_answers");
        correct_dropdown_answers.forEach(correct => {
            console.log("      " + correct);
        });
    }
    if (correct_select_answers != null) {

        console.log("Loop my");
        div_my_selection.forEach(my => {
            console.log("  " + my);
        });


        console.log("  Loop correct_select_answers");
        correct_select_answers.forEach(correct => {
            console.log("      " + correct);
        });
    }


    var allAllCorrect = true;

    var arrayWithIncorrect = [];
    var arrayWithCorrect = [];

    console.log("==> checkanswer kind:" + answer_kind);

    if (answer_kind == 3) //dropdown
    {
        console.log("[It is dropdown]");

        var div_all_dropdowns = document.querySelectorAll('[name="dropdown"]');
        console.log("Loop dropdown");
        
        div_all_dropdowns.forEach(dropdown => {

            dropdown.style.backgroundColor = 'white';

            var my_group = dropdown.id.substring(dropdown.id.length - 1, dropdown.id.length);
            var my_number = dropdown.selectedIndex;
            console.log("  my_group:" + my_group + "  my_number:" + my_number);
            if (my_number != 0)
            {
                var answer_is_correct = false;
                correct_dropdown_answers.forEach(correct => {
                    var correct_group = correct[0];
                    var correct_number = correct[1];
                    if (my_group == correct_group)
                    {
                        answer_is_correct = my_number == correct_number;
                    }
                    console.log("  correct_group:" + correct_group + "  correct_number:" + correct_number);
                });

                if (answer_is_correct == true) {
                    dropdown.style.backgroundColor = 'green';
                }
                else
                {
                    dropdown.style.backgroundColor = 'red';
                }

                console.log("  id:" + dropdown.id + "  value:" + dropdown.value + "  selectedIndex:" + dropdown.selectedIndex);
            }
        });
        return;
    }
    else if (answer_kind == 1 || answer_kind == 2) //Multi and Single-Select
    {
        console.log("[It is Multi/SingleSelect]");

        var div_all_single = document.querySelectorAll('#div_answers div');

        div_all_single.forEach(single => {
            single.style.backgroundColor = 'white';
        });

        console.log("Loop my");
        div_my_selection.forEach(my => {
            console.log("  " + my);

            var exists = false;
            correct_select_answers.forEach(correct => {
                if (correct == my)
                    exists = true;
            });
            document.querySelector('#div_answers div[name="' + my + '"]').style.backgroundColor = 'white';
            if (exists == true)
            {
                document.querySelector('#div_answers div[name="' + my + '"]').style.backgroundColor = 'green';
            }
            else
                document.querySelector('#div_answers div[name="' + my + '"]').style.backgroundColor = 'red';
            return;
        });

    }
    else if (answer_kind == 4) //Order
    {
        console.log("[It is Order]");

        div_my_selection.forEach(my => {
            document.querySelector('#div_answers div[name="' + my + '"]').style.backgroundColor = 'white';
        });

        var idx = 1;
        correct_select_answers.forEach(correct => {

            var select = document.querySelectorAll('select[name="selectoption"]');
            select.forEach(s => {
                var s_id = s.id;
                var s_value = s.value;

                console.log('  => id:' + s_id + ' value:' + s_value);
                console.log('  => correct:' + correct + ' idx:' + idx);
                if (s_id == correct && s_value == idx)
                {
                    document.querySelector('#div_answers div[name="' + s_id + '"]').style.backgroundColor = 'green';
                }

            });

            idx++;
        });

    }
};
