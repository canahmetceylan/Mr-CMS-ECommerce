(function () {
    $(function () {
        $(document).on('click', '[data-action="remove-searchable-page-hidden-specification"]', function (event) {
            event.preventDefault();
            var button = $(event.target);
            var attr = button.attr('href');
            var categoryId = $("#Id").val();
            $.post(attr, function (response) {
                $.get('/Admin/Apps/Ecommerce/EcommerceSearchablePage/HiddenSpecifications/' + categoryId, function (products) {
                    $('#hidden-specifications').replaceWith(products);
                });
            });
        });
    });
})();