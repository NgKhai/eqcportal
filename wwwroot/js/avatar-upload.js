(function () {
    const input = document.getElementById('profilePhoto');
    const preview = document.getElementById('avatarPreview');
    const placeholder = document.querySelector('.avatar-placeholder');
    const removeInput = document.getElementById('removePhoto');
    const removeButton = document.getElementById('removePhotoButton');

    if (input && preview && placeholder) {
        input.addEventListener('change', function () {
            const file = input.files && input.files[0];
            if (!file) {
                return;
            }

            preview.src = URL.createObjectURL(file);
            preview.classList.remove('d-none');
            placeholder.classList.add('d-none');

            if (removeInput) {
                removeInput.value = 'false';
            }
        });
    }

    if (removeButton && input && preview && placeholder && removeInput) {
        removeButton.addEventListener('click', function () {
            input.value = '';
            preview.removeAttribute('src');
            preview.classList.add('d-none');
            placeholder.classList.remove('d-none');
            removeInput.value = 'true';
        });
    }
})();
