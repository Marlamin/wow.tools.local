window.flagMap = new Map();
window.enumMap = new Map();
window.colorFields = [];
window.dateFields = [];
window.conditionalFKs = new Map();
window.conditionalEnums = new Map();
window.conditionalFlags = new Map();

// Load the mappings if it hasn't been loaded yet
window.onload = _ => {
    fetch('/dbc/meta/getMappings')
        .then(res => res.json())
        .then(data => {
            data.forEach(entry => {
                let tableColumnKey = `${entry.tableName.toLowerCase()}.${entry.columnName}`;
                if (entry.arrIndex !== null) {
                    tableColumnKey += `[${entry.arrIndex}]`;
                }

                const isConditional = entry.conditionalTable && entry.conditionalColumn;
                if (entry.meta === 0) {         // Flags
                    window.flagMap.set(tableColumnKey, entry.entries);

                    if (isConditional) {
                        if (!window.conditionalFlags.has(tableColumnKey)) {
                            window.conditionalFlags.set(tableColumnKey, []);
                        }
                        const conditionKey = `${entry.conditionalTable.toLowerCase()}.${entry.conditionalColumn}=${entry.conditionalValue}`;
                        window.conditionalFlags.get(tableColumnKey).push([conditionKey, entry.entries]);
                    }

                } else if (entry.meta === 1) {  // Enum
                    window.enumMap.set(tableColumnKey, entry.entries);

                    if (isConditional) {
                        if (!window.conditionalEnums.has(tableColumnKey)) {
                            window.conditionalEnums.set(tableColumnKey, []);
                        }
                        const conditionKey = `${entry.conditionalTable.toLowerCase()}.${entry.conditionalColumn}=${entry.conditionalValue}`;
                        window.conditionalEnums.get(tableColumnKey).push([conditionKey, entry.entries]);
                    }
                } else if (entry.meta === 2) {  // Color
                    window.colorFields.push(tableColumnKey);
                } else if (entry.meta === 3) {  // Date
                    window.dateFields.push(tableColumnKey);
                }

                // console.log(entry);
            });
        });
};

function getEnum(db, field, value) {
    // eslint-disable-next-line no-undef
    let targetEnum = window.enumMap.get(db.toLowerCase() + '.' + field);
    return getEnumVal(targetEnum, value);
}

function getEnumVal(targetEnum, value) {
    if (targetEnum == null) {
        return "Unk";
    }

    const targetEnumEntry = targetEnum.find(item => item.value == value);
    if (targetEnumEntry != null) {
        if (targetEnumEntry.name == null) {
            return "Unk";
        } else {
            return `${targetEnumEntry.name}`;
        }

        // if (Array.isArray(targetEnum[value])){
        //     return targetEnum.at(value)[0].name;
        // } else {
        //     return targetEnum.at(value).name;
        // }
    } else {
        return "Unk";
    }
}
