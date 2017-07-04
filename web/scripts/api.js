function API(baseUri) {
    normalizeBaseUrl();

    var self = {};
    self.cards = {
        GET: getCards,
        PUT: updateCards
    };

    return self;

    function getCards(projectName, areaName, depth) {
        return get(cardsUri(projectName, areaName) + queryString(depth));
    }

    function queryString(depth) {
        return depth ? '?depth=' + depth : '';
    }

    function updateCards(projectName, areaName, cards) {
        return put(cardsUri(projectName, areaName), cards);
    }

    function get(uri) {
        return call('get', uri);
    }

    function put(uri, data) {
        return call('put', uri, data);
    }

    function call(verb, uri, data) {
        return $.ajax({
            url: uri,
            type: verb,
            data: JSON.stringify(data),
            contentType: 'application/json; charset=UTF-8'
        });
    }

    function dataType(data) {
        return data == undefined
            ? undefined
            : 'application/json';
    }

    function cardsUri(projectName, areaName) {
        return [baseUri, "api/projects", projectName, areaName, "cards"].join('/')
    }

    function normalizeBaseUrl() {
        if (baseUri == '/')
            baseUri = undefined;
    }
}