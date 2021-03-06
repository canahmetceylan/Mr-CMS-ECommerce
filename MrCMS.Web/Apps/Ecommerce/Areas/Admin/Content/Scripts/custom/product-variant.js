$(document).ready(function () {
    
    //this will validate hidden fields required for IsDownloadable
    $.validator.setDefaults({
        ignore: [],
        // any other default options and/or rules
    });

    function formatImageOption(state) {
        var option = state.element;
        var url = $(option).data('url');
        $("#FeaturedImageUrl").val($(option).data('url'));
        if (url != 0) {
            return "<table><tr><td><img style='width:80px;max-height:50px;' src='" + url + "' " +
                "alt='" + $(option).data('name') + "' /></td><td style='padding-left:10px'>" + state.text + "</td></tr></table>";
        } else {
            return "<p style='padding-top:15px'>" + state.text+"</p>";
        }
    }

    $("#FeaturedImageUrlBox").select2({
        formatResult: formatImageOption,
        formatSelection: formatImageOption,
        escapeMarkup: function(m) { return m; }
    });
    
    $('#SoldOut').click(function () {
        $("#sold-out-message").toggle(this.checked);
    });
    
    function formatOption(state) {
        return state.text;
    }

    $("#ShippingMethodsBox").select2({
        formatResult: formatOption,
        formatSelection: formatOption,
        escapeMarkup: function (m) { return m; }
    });

    $("#ShippingMethodsBox").on("change",
        function(e) {
            $("#ShippingMethodsValue").val($("#ShippingMethodsBox").select2("val"));
        });
    
    $('#IsDownloadable').click(function () {
        $('#DownloadFileUrl').val('');
        $('#DemoFileUrl').val('');
        $('#AllowedNumberOfDownloads').val('0');
        $('#AllowedNumberOfDaysForDownload').val('0');
        $("#downloadable").toggle(this.checked);
    });

    $(document).on('change', '#VariantType', function (event) {
        var value = $(event.target).val();
        var variantType = $('[data-variant-type]');
        var selector = '[data-variant-type="' + value + '"]';
        variantType.filter(selector).show();
        variantType.not(selector).hide();
    });
    $(document).on('change', '#RequiresShipping', function (event) {
        event.preventDefault();
        $("[data-requires-shipping]").toggle(this.checked);
    });
    $(document).on('change', '#HasRestrictedShipping', function (event) {
        event.preventDefault();
        $("[data-has-restricted-shipping]").toggle(this.checked);
    });
    $('#RequiresShipping, #HasRestrictedShipping, #VariantType').change();
});
