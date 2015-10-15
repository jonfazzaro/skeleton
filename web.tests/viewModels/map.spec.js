/// <reference path="../../web/scripts/map.js" />

describe("The map view model", function () {

    beforeEach(function () {
        _model = new mapViewModel(_api, _resources);
    });

    it("has a list of cards", function () {
        expect(_model.cards()).toEqual([]);
    });

    describe("when loaded", function () {
        beforeEach(function () {
            spyOn(_api.cards, 'GET').and.returnValue(success(_storyData));
            spyOn(toastr, 'error').and.callFake(function () { });
            spyOn(toastr, 'success').and.callFake(function () { });
            spyOn(console, 'log').and.callFake(function () { });
            _model.isDirty(true);
            _model.load('yeah yeah');
        });

        it("calls the cards API with the project name", function () {
            var depth;
            expect(_api.cards.GET).toHaveBeenCalledWith('yeah yeah', depth);
        });

        describe("when loaded with a depth", function () {
            beforeEach(function () {
                _model.load('yeah yeah', 17);
            });

            it("calls the cards API with the project name and the depth", function () {
                expect(_api.cards.GET).toHaveBeenCalledWith('yeah yeah', 17);
            });
        });

        it("gets the cards from the API", function () {
            expect(_model.cards()).toEqual(_storyData);
        });

        it("hides the loading indicator", function () {
            expect(_model.isLoading()).toEqual(false);
        });

        it("unsets the dirty flag", function () {
            expect(_model.isDirty()).toEqual(false);
        });

        it("gets a list of features", function () {
            expect(_model.features(_storyData)).toEqual([
                _storyData[0],
                _storyData[1],
                _storyData[2]
            ]);
        });

        it("gets a list of stories", function () {
            expect(_model.stories(_storyData)).toEqual([
                _storyData[3],
                _storyData[4],
                _storyData[5]
            ]);
        });

        describe("given a card has no children", function () {
            it("gets an empty list for its children", function () {
                expect(_model.children(_model.cards()[0])).toEqual([]);
            });
        });

        describe("given a card has children", function () {
            it("lists it", function () {
                expect(_model.parents(_storyData)).toEqual([
                    _storyData[2]
                ]);
            });
        });
        
        describe("given a card has children", function () {
            var _children;
            beforeEach(function () {
                _children = _model.children(_model.cards()[2]);
            });

            it("gets its children sorted by priority", function () {
                expect(_children).toEqual([
                    _storyData[5],
                    _storyData[4],
                    _storyData[3]
                ]);
            });
        });
        
        describe("given the call has not yet returned", function () {
            beforeEach(function () {
                _api.cards.GET.and.returnValue(waiting());
                _model.load('yeah yeah');
            });

            it("shows the loading indicator", function () {
                expect(_model.isLoading()).toEqual(true);
            });
        });

        describe("given the API call fails", function () {
            beforeEach(function () {
                _api.cards.GET.and.returnValue(failure("wat"));
                _model.load('yo yo');
            });

            it("shows an error message", function () {
                expect(toastr.error).toHaveBeenCalledWith(_resources.strings.errorMessage);
            });

            it("logs the error response", function () {
                expect(console.log).toHaveBeenCalledWith("wat");
            });

            it("hides the loading indicator", function () {
                expect(_model.isLoading()).toEqual(false);
            });
        });

        describe("when the user changes the sort order", function () {
            beforeEach(function () {
                $('.feature').remove();
                $('body').append(_fakeDOM);
                _model.reindex();
            });

            it("indexes the cards horizontally", function () {
                expect(byId(_model.cards(), 1).Priority).toEqual(10);
                expect(byId(_model.cards(), 4).Priority).toEqual(20);
                expect(byId(_model.cards(), 7).Priority).toEqual(40);
                expect(byId(_model.cards(), 13).Priority).toEqual(50);
                expect(byId(_model.cards(), 176).Priority).toEqual(60);
                expect(byId(_model.cards(), 2).Priority).toEqual(30);
            });

            it("sets the dirty flag", function () {
                expect(_model.isDirty()).toEqual(true);
            });
        });

        describe("when the user saves", function () {
            beforeEach(function () {
                spyOn(_api.cards, 'PUT').and.returnValue(waiting());
                _model.save();
            });

            it("sets the loading flag", function () {
                expect(_model.isLoading()).toEqual(true);
            });

            it("puts to the cards API", function () {
                expect(_api.cards.PUT).toHaveBeenCalledWith('yeah yeah', _model.cards());
            });

            describe("given the API call fails", function () {
                beforeEach(function () {
                    _api.cards.PUT.and.returnValue(failure('oh noes'));
                    _model.save();
                });

                it("handles the error", function () {
                    expect(console.log).toHaveBeenCalledWith('oh noes');
                    expect(toastr.error).toHaveBeenCalledWith(_resources.strings.errorMessage);
                });

                it("unsets the loading flag", function () {
                    expect(_model.isLoading()).toEqual(false);
                });
            });

            describe("given the API call succeeds", function () {
                beforeEach(function () {
                    _api.cards.PUT.and.returnValue(success());
                    _model.isDirty(true);
                    _model.save();
                });

                it("unsets the loading flag", function () {
                    expect(_model.isLoading()).toEqual(false);
                });

                it("unsets the dirty flag", function () {
                    expect(_model.isDirty()).toEqual(false);
                });

                it("toasts to the users success", function () {
                    expect(toastr.success).toHaveBeenCalledWith(_resources.strings.savedMessage);
                });
            });
        });
    });

    var _model;
    var _resources = new Resources();
    var _api = {
        cards: {
            GET: function () { },
            PUT: function () { }
        }
    }

    function byId(items, id) {
        return items.filter(function(i) {
            return i.Id == id;
        })[0];
    }

    function by(field) {
        return function (s) {
            return s[field];
        };
    }

    function success(data) {
        var promise = $.Deferred();
        promise.resolve(data);
        return promise;
    }

    function waiting() {
        var promise = $.Deferred();
        return promise;
    }

    function failure(res) {
        var promise = $.Deferred();
        promise.reject(res);
        return promise;
    }

    var _storyData = [
        { Id: 1, Priority: 345, Type: 'Feature', FeatureId: null, ParentId: null },
        { Id: 2, Priority: 123, Type: 'Feature', FeatureId: null, ParentId: null },
        { Id: 4, Priority: 987, Type: 'Feature', FeatureId: null, ParentId: null },
        { Id: 7, Priority: 6543, Type: 'User Story', FeatureId: 4, ParentId: 4 },
        { Id: 13, Priority: 4321, Type: 'Product Backlog Item', FeatureId: 4, ParentId: 4 },
        { Id: 176, Priority: 124, Type: 'Product Backlog Item', FeatureId: 4, ParentId: 4 }
    ];

    var _fakeDOM =
        '<div class="feature">       ' +
        '    <div id="1" class="card"></div>' + // 10
        '</div>                      ' + 
        '<div class="feature">       ' + 
        '    <div id="4" class="card"></div>' + // 20
        '    <div id="7" class="card"></div>' + // 40
        '    <div id="13" class="card"></div>' + // 50
        '    <div id="176" class="card"></div>' + // 60
        '</div>"' +
        '<div class="feature">       ' + 
        '    <div id="2" class="card"></div>' + // 30
        '</div>';
});