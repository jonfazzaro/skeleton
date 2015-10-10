(function () {
    ko.bindingHandlers.sortable = {
        init: function (element, valueAccessor) {
            var options = getOptions(valueAccessor());
            Sortable.create(element, options);
        }
    }

    function getOptions(values) {
        return $.extend({
            animation: 500
        }, values);
    }
})();