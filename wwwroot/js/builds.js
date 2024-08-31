$(document).ready(function () {
    let build1;
    let build2;

    function resetDiffs() {
        build1 = null;
        build2 = null;
        $('.diffbadge').remove();
        $('#openDiffButton').hide();
        $('#openInstallDiffButton').hide();
        $('#resetButton').hide();
        $('#diffButton').show();
        $('#buildtable tbody').off('click', 'tr', onBuildClick);
    }

    $('#diffButton').on('click', function () {
        $('#buildtable tbody').on('click', 'tr', onBuildClick);
        $('#diffButton').hide();
        $('#resetButton').show();
        $('#openDiffButton')
            .attr('href', '#')
            .text('Click the row of the first build (old)')
            .show();

        return false;
    }).removeClass('disabled');

    $('#resetButton').on('click', function () {
        resetDiffs();
        return false;
    });
    $('#openDiffButton').on('click', function () {
        if (!build1 || !build2) {
            return false;
        }

        resetDiffs();
    });

    $('#openInstallDiffButton').on('click', function () {
        if (!build1 || !build2) {
            return false;
        }

        openInstallDiff();
    });

    function onBuildClick() {
        const hashElement = $(this).find('.buildconfighash');

        if (!build1) {
            build1 = hashElement.text();
            hashElement.after(' <span class="badge bg-danger diffbadge">Old build</span>');
            $('#openDiffButton').text('Click the row of the second build (new)');
        } else if (!build2) {
            build2 = hashElement.text();
            hashElement.after(' <span class="badge bg-danger diffbadge">New build</span>');
            $('#openDiffButton')
                .text('Click to diff (might take up to a minute to generate)')
                .attr('href', '/builds/diff.html?from=' + build1 + '&to=' + build2);
            //$('#openInstallDiffButton').show();
        }
    }

    function openInstallDiff() {
        $("#installDiffModal").modal('show');
        $("#installDiffModalContent").html("Generating diff..");

        var fromArray = [];
        var toArray = [];

        var added = [];
        var modified = [];
        var removed = [];

        var result = "<table>";

        $.getJSON("https://wow.tools/casc/install/dumpbybuild?buildConfigHash=" + build1, function (fromData) {
            $.getJSON("https://wow.tools/casc/install/dumpbybuild?buildConfigHash=" + build2, function (toData) {
                $.each(fromData, function (key, val) {
                    fromArray[val.name] = val;
                });

                $.each(toData, function (key, val) {
                    toArray[val.name] = val;
                });

                $.each(fromData, function (key, val) {
                    if (val.name in toArray) {
                        if (val.contentHash != toArray[val.name].contentHash) {
                            modified.push(val);
                        }
                    } else {
                        removed.push(val);
                    }
                });

                $.each(toData, function (key, val) {
                    if (!(val.name in fromArray)) {
                        added.push(val);
                    }
                });

                added.forEach(function (val) {
                    result += "<tr><td><span class='badge bg-success'>Added</span></td><td>" + val.name + "</td></tr>";
                });
                modified.forEach(function (val) {
                    result += "<tr><td><span class='badge bg-warning'>Modified</span></td><td>" + val.name + "</td></tr>";
                });
                removed.forEach(function (val) {
                    result += "<tr><td><span class='badge bg-danger'>Removed</span></td><td>" + val.name + "</td></tr>";
                });

                result += "</table>";

                $("#installDiffModalContent").html(result);
            });
        });

        return false;
    }
});

function fillVersionModal(id) {
    $("#moreInfoModalContent").load("/builds/index.php?api=buildinfo&versionid=" + id);
}

function fillConfigModal(config) {
    $("#configModalContent").load("/builds/index.php?api=configdump&config=" + config);
}

$(function () {
    $('[data-bs-toggle="popover"]').popover()
})