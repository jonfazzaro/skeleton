function API(baseUri) {
    normalizeBaseUrl();

    var self = {};
    self.cards = {
        GET: getCards,
        PUT: updateCards
    };

    return self;

    function getCards(projectName, depth) {
        return get(cardsUri(projectName) + queryString(depth));
    }

    function queryString(depth) {
        return depth ? '?depth=' + depth : '';
    }

    function updateCards(projectName, cards) {
        return put(cardsUri(projectName), cards);
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

    function cardsUri(projectName) {
        return [baseUri, "api/projects", projectName, "cards"].join('/')
    }

    function normalizeBaseUrl() {
        if (baseUri == '/')
            baseUri = undefined;
    }
}