let index = 0;

function AddTag() {
    // Get a reference to the TagEntry input element
    let tagEntry = document.getElementById("TagEntry");

    // Create a new Select Option
    let newOption = new Option(tagEntry.value, tagEntry.value);

    document.getElementById("TagList").options[index++] = newOption;

    // Clear the TagEntry control
    tagEntry.value = "";

    return true;
}

function DeleteTag() {
    let tagCount = 1;

    while (tagCount > 0) {
        let tagList = document.getElementById("TagList")
        let selectedIndex = tagList.selectedIndex;

        if (selectedIndex >= 0) {
            tagList.options[selectedIndex] = null;
            --tagCount;
        } else {
            tagCount = 0;
        }

        index--;
    }
}

function onSubmit () {
    let select = document.getElementById("TagList").getElementsByTagName("option");

    for (let option of select) {
        option.selected = "selected";
    }
}

document.getElementById("Form").addEventListener("submit", onSubmit)