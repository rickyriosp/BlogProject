let index = 0;

function AddTag() {
    // Get a reference to the TagEntry input element
    let tagEntry = document.getElementById("TagEntry");

    // Create a new Select Option
    let newOption = new Option(tagEntry.value, tagEntry.value);

    document.getElementById("TagList").options[index++] = newOption;

    // Clear the TagEntry control
    tagEntry.value = '';

    return true;
}

function DeleteTag() {

}