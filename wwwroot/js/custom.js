let index = 0;

// Look for the tagValues variable to see if it has data
if (tagValues != '') {
    let tagArray = tagValues.split(",");
    index = tagArray.length;
}

function AddTag() {
    // Get a reference to the TagEntry input element
    let tagEntry = document.getElementById("TagEntry");

    // Use the new Search function to help detect an error state
    let searchResult = Search(tagEntry.value);

    if (searchResult != null) {
        // Trigger Sweet Alert for the error result
        swalWithDarkButton.fire({
            html: `<span class="font-weight-bolder">${searchResult.toUpperCase()}</span>`
        })
    } else {
        // Create a new Select Option
        let newOption = new Option(tagEntry.value, tagEntry.value);

        document.getElementById("TagList").options[index++] = newOption;
    }

    // Clear the TagEntry control
    tagEntry.value = "";
    tagEntry.focus();

    return true;
}

function DeleteTag() {
    let tagCount = 1;

    let tagList = document.getElementById("TagList");
    if (!tagList) return false;

    if (tagList.selectedIndex == -1) {
        swalWithDarkButton.fire({
            html: '<span class="font-weight-bolder">CHOOSE A TAG BEFORE DELETING</span>'
        })

        return true;
    }

    while (tagCount > 0) {
        if (tagList.selectedIndex >= 0) {
            tagList.options[tagList.selectedIndex] = null;
            --tagCount;
        } else {
            tagCount = 0;
        }
    }
}

function onSubmit () {
    let select = document.getElementById("TagList").getElementsByTagName("option");

    for (let option of select) {
        option.selected = "selected";
    }
}

document.getElementById("Form").addEventListener("submit", onSubmit)

// The Search function will detect either an empty or a duplicate Tag
// and return an error string if an error is detected
function Search(str) {
    // Empty Tag
    if (str === "") {
        return "Empty tags are note permitted";
    }

    let tagsEl = document.getElementById("TagList");

    // Duplicate Tag
    if (tagsEl) {
        let options = tagsEl.options;

        for (let index = 0; index < options.length; index++) {
            if (options[index].value == str) {
                return `The Tag #${str} is not permitted because is a duplicate`;
            }
        }
    }
}

const swalWithDarkButton = Swal.mixin({
    customClass: {
        popup: "swal2popup",
        actions: "w-75",
        confirmButton: 'btn btn-danger d-grid btn-outline-dark w-100',
    },
    imageUrl: "/assets/img/oops.png",
    timer: 3000,
    confirmButtonText: 'Close',
    buttonsStyling: false,
})