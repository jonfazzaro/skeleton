function mapViewModel(api, resources) {

    self = {};
    self.cards = ko.observableArray();
    self.load = load;
    self.sort = sort;
    self.features = features;
    self.stories = stories;
    self.children = children;
    self.parents = parents;
    self.reindex = reindex;
    self.isDirty = ko.observable(false);
    self.save = save;
    self.isLoading = ko.observable(false);
    var _projectName;
    var _countBy = 10;
    return self;

    function load(projectName, depth) {
        _projectName = projectName;
        self.isLoading(true);
        return api.cards.GET(projectName, depth)
            .done(loadCards)
            .fail(handleError)
            .always(stopLoading);
    };

    function sort(items) {
        return items.sort(by('Priority'));
    }

    function by(field) {
        return function (a, b) {
            return a[field] - b[field];
        };
    }

    function handleError(error) {
        console.log(error);
        toastr.error(resources.strings.errorMessage);
    }

    function save() {
        self.isLoading(true);
        return api.cards.PUT(_projectName, self.cards())
            .done(saved)
            .fail(handleError)
            .always(stopLoading);
    }

    function reindex() {
        var index = next(index);
        var features = $('.feature');
        for (var c = 0; c < self.cards().length; c++) {
            for (var f = 0; f < features.length; f++) {
                var cards = $('.card', features[f]);
                var card = getCardById($(cards[c]).attr('id'));
                if (card) {
                    card.Priority = index;
                    index = next(index);
                    self.isDirty(true);
                }
            }
        }
    }

    function getCardById(id) {
        return self.cards().filter(function (c) {
            return c.Id == id;
        })[0];
    }

    function next(index) {
        return (index || 0) + _countBy;
    }

    function children(card) {
        return sort(stories(self.cards().filter(byParentId(card.Id))));
    }

    function parents(cards) {
        return cards.filter(function (c) {
            return children(c).length > 0;
        });
    }

    function byParentId(id) {
        return function (c) {
            return c.ParentId == id;
        };
    }

    function features(cards) {
        return cards.filter(function (c) {
            return c.Type == 'Feature';
        })
    }

    function stories(cards) {
        return cards.filter(function (c) {
            return c.Type != 'Feature';
        })
    }

    function stopLoading() {
        self.isLoading(false);
    }

    function loadCards(data) {
        self.cards(data);
        self.isDirty(false);
    }

    function saved() {
        self.isDirty(false);
        toastr.success(resources.strings.savedMessage);
    }
}