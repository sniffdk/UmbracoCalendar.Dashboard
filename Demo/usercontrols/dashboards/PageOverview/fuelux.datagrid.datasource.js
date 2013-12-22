/*
 * Fuel UX Data components - static data source
 * https://github.com/ExactTarget/fuelux-data
 *
 * Copyright (c) 2012 ExactTarget
 * Licensed under the MIT license.
 */

(function (root, factory) {
    if (typeof define === 'function' && define.amd) {
        define(['underscore'], factory);
    } else {
        root.StaticDataSource = factory();
    }
}(this, function () {

    var StaticDataSource = function (options) {
        this._formatter = options.formatter;
        this._columns = options.columns;
        this._delay = options.delay || 0;
        this._data = options.data;
    };

    StaticDataSource.prototype = {

        columns: function () {
            return this._columns;
        },

        data: function (options, callback) {
            var self = this;

            setTimeout(function () {
                var data = $.extend(true, [], self._data);

                // SEARCHING
                if (options.search) {
                    data = _.filter(data, function (item) {
                        for (var prop in item) {
                            if (!item.hasOwnProperty(prop)) continue;
                            if (~item[prop].toString().toLowerCase().indexOf(options.search.toLowerCase())) return true;
                        }
                        return false;
                    });
                }

                // FILTERING
                if (options.filter) {
                    data = _.filter(data, function (item) {
                        var match;
                        switch (options.filter.value) {
                            case "all":
                                match = true;
                                break;
                            default:
                                match = item.MyPage === true;
                                break;
                        }
                        return match;
                    });
                }

                var count = data.length;

                // SORTING
                if (options.sortProperty) {
                    if (options.sortProperty === "Updated") {
                        data = _.sortBy(data, function (item) {
                            var parts = item.Updated.split(' ');
                            var dateParts = parts[0].split('-');
                            var timeParts = parts[1].split(':');
                            var date = new Date(dateParts[2], dateParts[1] - 1, dateParts[0], timeParts[0], timeParts[1]);
                            return date.getTime();
                        });
                    } else {
                        data = _.sortBy(data, options.sortProperty);
                    }

                    if (options.sortDirection === 'desc') data.reverse();
                }

                // PAGING
                var startIndex = options.pageIndex * options.pageSize;
                var endIndex = startIndex + options.pageSize;
                var end = (endIndex > count) ? count : endIndex;
                var pages = Math.ceil(count / options.pageSize);
                var page = options.pageIndex + 1;
                var start = startIndex + 1;

                data = data.slice(startIndex, endIndex);

                if (self._formatter) self._formatter(data);

                callback({ data: data, start: start, end: end, count: count, pages: pages, page: page });

            }, this._delay)
        }
    };

    return StaticDataSource;
}));
