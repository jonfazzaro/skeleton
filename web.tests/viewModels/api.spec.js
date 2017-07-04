/// <reference path="../../web/scripts/api.js" />
describe("The api service", function () {
    var _result;
    beforeEach(function () {
        spyOn($, 'ajax').and.callFake(captureOptions)
        _api = new API('base.uri.com');
    });

    describe("when getting cards", function () {

        beforeEach(function () {
            _result = _api.cards.GET('mahProject', 'mahArea');
        });

        it("calls the cards API", function () {
            expect(_options.url).toEqual("base.uri.com/api/projects/mahProject/mahArea/cards");
            expect(_options.type).toEqual("get");
        });

        describe("given a depth", function () {
            beforeEach(function () {
                _result = _api.cards.GET('mahProject', 'mahArea', 12);
            });

            it("calls the cards API", function () {
                expect(_options.url).toEqual("base.uri.com/api/projects/mahProject/mahArea/cards?depth=12");
                expect(_options.type).toEqual("get");
            });
        });

        andThisStuffToo();
    });
    
    describe("when updating cards", function () {

        beforeEach(function () {
            _cards = [{id:2},{id:5}];
            _result = _api.cards.PUT('yoProject', 'yoArea', _cards);
        });

        it("calls the cards API", function () {
            expect(_options.url).toEqual("base.uri.com/api/projects/yoProject/yoArea/cards");
            expect(_options.type).toEqual("put");
            expect(_options.data).toBe(JSON.stringify(_cards));
        });

        it("returns a promise", function () {
            expect(typeof _result.done).toEqual("function");
        });

        andThisStuffToo();
    });

    function andThisStuffToo() {

        it("returns a promise", function () {
            expect(typeof _result.done).toEqual("function");
        });

        describe("given no base uri", function () {
            beforeEach(function () {
                _api = new API();
                _result = _api.cards.GET('mahProject', 'mahArea');
            });

            it("uses the root", function () {
                expect(_options.url).toEqual("/api/projects/mahProject/mahArea/cards");
            });
        });

        describe("given a base uri of '/'", function () {
            beforeEach(function () {
                _api = new API('/');
                _result = _api.cards.GET('mahProject', 'mahArea');
            });

            it("uses the root", function () {
                expect(_options.url).toEqual("/api/projects/mahProject/mahArea/cards");
            });
        });
    }


    var _options;
    var _api;

    function captureOptions(options) {
        _options = options;
        return $.Deferred();
    }
});