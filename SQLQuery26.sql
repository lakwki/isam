  select  distinct a.contractno + '-' + convert(varchar,convert(int, a.deliveryno) )
  from ilsorderref a inner join ilsinvoice b on a.orderrefid = b.orderrefid and  a.contractno + '-' + deliveryno in (
'OG8484563-01',
'OG8484563-01',
'OG8484563-01',
'OG8484563-01',
'OG8484563-01',
'OG8484563-01',
'OG8484563-01',
'OG8484563-01',
'OG8484563-01',
'OG8484563-01',
'OG8484576-01',
'OG8484576-01',
'OG8484576-01',
'OG8484576-01',
'OG8484576-01',
'OG8484576-01',
'OG8484576-01',
'OG8484576-01',
'OG8484576-01',
'OG8484576-01',
'OG8484576-01',
'OG8484576-01',
'OG8484576-01',
'OG8484576-01',
'OG8484741-02',
'OG8484741-02',
'OG8484741-02',
'OG8484741-02',
'OG8484741-02',
'OG8484741-02',
'OG8484741-02',
'OG8484741-02',
'OG8484741-02',
'OG8484741-02',
'OG8484741-02',
'OG8484741-02',
'OG8484741-02',
'OG8482691-02',
'OG8482691-02',
'OG8482691-02',
'OG8482691-02',
'OG8482691-02',
'OG8482691-02',
'OG8482691-02',
'OG8482691-02',
'OG8482691-02',
'OG8483302-03',
'OG8483302-03',
'OG8483302-03',
'OG8483302-03',
'OG8483302-03',
'OG8483302-03',
'OG8483302-03',
'OG8483302-03',
'OG8483302-03',
'OG8485067-01',
'OG8485067-01',
'OG8485067-01',
'OG8485067-01',
'OG8485067-01',
'OG8485067-01',
'OG8485067-01',
'OG8485067-01',
'OG8485067-01',
'OG8485067-01',
'OG8485067-01',
'OG8485067-01',
'OG8485067-01',
'UH8478786-05',
'UH8478786-05',
'UH8478786-05',
'UH8478786-05',
'UH8478786-05',
'UH8478786-05',
'UH8478786-05',
'UH8478786-05',
'UH8478786-05',
'UH8478786-03',
'UH8478786-03',
'UH8478786-03',
'UH8478786-03',
'UH8478786-03',
'UH8478786-03',
'UH8478786-03',
'UH8478786-03',
'UH8478786-03',
'MI8492843-04',
'MI8492843-04',
'MI8492843-04',
'MI8492843-04',
'MI8492843-04',
'MI8492843-04',
'MI8492843-04',
'MI8492843-04',
'MI8492843-04',
'OG8484699-01',
'OG8484699-01',
'OG8484699-01',
'OG8484699-01',
'OG8484699-01',
'OG8484699-01',
'OG8484699-01',
'OG8484699-01',
'OG8484699-01',
'OG8484709-01',
'OG8484709-01',
'OG8484709-01',
'OG8484709-01',
'OG8484709-01',
'OG8484709-01',
'OG8484709-01',
'OG8484709-01',
'OG8484709-01',
'OG8484709-01',
'OG8484709-01',
'OG8484709-01',
'OG8484709-01',
'OG8484709-01',
'OG8484880-01',
'OG8484880-01',
'OG8484880-01',
'OG8484880-01',
'OG8484880-01',
'OG8484880-01',
'OG8484880-01',
'OG8484880-01',
'OG8484880-01',
'OG8484880-01',
'OG8484880-01',
'OG8484880-01',
'OG8484880-01',
'OG8484880-01',
'UH8476254-03',
'UH8476254-03',
'UH8476254-03',
'UH8476254-03',
'UH8478786-04',
'UH8478786-04',
'UH8478786-04',
'UH8478786-04',
'UH8478786-04',
'UH8478786-04',
'UH8478786-04',
'UH8478786-04',
'UH8478786-04',
'VA8479196-01',
'VQ8465964-01',
'VA8479581-01',
'VI8478058-01',
'VI8478058-01',
'VI8478058-01',
'VI8478058-01',
'VI8478058-01',
'VI8477923-01',
'VI8477923-01',
'VI8477923-01',
'VI8477923-01',
'VI8477923-01',
'VI8477923-01',
'VQ8466138-01',
'VQ8466138-01',
'VQ8466138-01',
'VQ8466138-01',
'VQ8466138-01',
'VB8491695-02',
'VB8491695-02',
'VB8491695-02',
'VB8491695-02',
'VB8491695-02',
'VB8491695-02',
'VJ8492830-01',
'VJ8492830-01',
'VJ8492830-01',
'VJ8492830-01',
'VJ8492830-01',
'VJ8492830-01',
'VJ8492830-01',
'MF8452582-02',
'MF8452582-02',
'MF8452582-02',
'MF8452582-02',
'MF8452582-02',
'MF8452582-02',
'MF8452582-02',
'MF8452582-02',
'MF8452582-02',
'VJ8492226-04',
'YB8476416-01',
'YB8476416-01',
'YB8476416-01',
'YB8476416-01',
'YB8476416-01',
'YB8476416-01',
'YB8476416-01',
'YB8476416-01',
'YB8476416-01',
'YB8476416-01',
'YB8476416-01',
'YB8476416-01',
'YB8476416-01',
'YB8476416-01',
'VJ8492830-03',
'VJ8492830-03',
'VJ8492830-03',
'VJ8492830-03',
'VJ8492830-03',
'VJ8492830-03',
'VB8491718-02',
'VB8491718-02',
'VB8491718-02',
'VB8491718-02',
'VB8491718-02',
'VB8491718-02',
'VB8491718-02',
'YB8476403-01',
'YB8476403-01',
'YB8476403-01',
'YB8476403-01',
'YB8476403-01',
'YB8476403-01',
'YB8476403-01',
'YB8476403-01',
'YB8476403-01',
'YB8476403-01',
'AJ8433734-01',
'AJ8433734-01',
'AJ8433734-01',
'AJ8433734-01',
'AJ8433734-01',
'AJ8433734-01',
'AJ8433734-01',
'AJ8433734-01',
'AJ8433721-01',
'AJ8433721-01',
'AJ8433721-01',
'AJ8433721-01',
'AJ8433721-01',
'AJ8433721-01',
'AJ8433721-01',
'AJ8433721-01',
'AJ8434461-01',
'AJ8434461-01',
'AJ8434461-01',
'AJ8434461-01',
'AJ8434461-01',
'AJ8434461-01',
'AJ8434461-01',
'AJ8434461-01',
'AJ8434461-01',
'MB8474023-03',
'MB8474023-03',
'MB8474023-03',
'MB8474023-03',
'MB8474023-03',
'MB8474023-03',
'MB8474023-03',
'MB8474023-03',
'MB8474023-03',
'MB8477075-01',
'MB8477075-01',
'MB8477075-01',
'MB8477075-01',
'MB8477075-01',
'MB8477075-01',
'MB8477075-01',
'MB8477075-01',
'MB8477075-01',
'AJ8433734-01',
'AJ8433734-01',
'AJ8433734-01',
'AJ8433734-01',
'AJ8433734-01',
'AJ8433734-01',
'AJ8433734-01',
'AJ8433734-01',
'AJ8433721-01',
'AJ8433721-01',
'AJ8433721-01',
'AJ8433721-01',
'AJ8433721-01',
'AJ8433721-01',
'AJ8433721-01',
'AJ8433721-01',
'AJ8434461-01',
'AJ8434461-01',
'AJ8434461-01',
'AJ8434461-01',
'AJ8434461-01',
'AJ8434461-01',
'AJ8434461-01',
'AJ8434461-01',
'AJ8434461-01',
'YB8477431-01',
'YB8477431-01',
'YB8477431-01',
'YB8477431-01',
'YB8477431-01',
'YB8477431-01',
'YB8477431-01',
'YB8477431-01',
'YB8477431-01',
'PA8452236-01',
'PA8452236-01',
'PA8452236-01',
'PA8452236-01',
'PA8452236-01',
'PA8452236-01',
'PA8452236-01',
'PA8452236-01',
'PA8452236-01',
'OF8472397-01',
'OF8472397-01',
'OF8472397-01',
'OF8472397-01',
'OF8472397-01',
'OF8472397-01',
'OF8472397-01',
'OF8472397-01',
'OF8472397-01',
'OF8472397-01',
'OF8472397-01',
'OF8472397-01',
'OF8472397-01',
'OF8472397-01',
'OF8472384-01',
'OF8472384-01',
'OF8472384-01',
'OF8472384-01',
'OF8472384-01',
'OF8472384-01',
'OF8472384-01',
'OF8472384-01',
'OF8472384-01',
'OF8472384-01',
'VA8482963-01',
'VA8482963-01',
'VA8482963-01',
'VA8482963-01',
'VA8482963-01',
'VA8482963-01',
'VA8482963-01',
'VA8482963-01',
'AD8575689-02',
'VA8478935-02',
'VA8478935-02',
'VA8478935-02',
'VA8478935-02',
'VA8478935-02',
'VA8478935-02',
'VA8478935-02',
'VA8478935-02',
'VA8480253-03',
'VA8480253-03',
'VA8480253-03',
'VA8480253-03',
'VA8480253-03',
'VA8480253-03',
'VA8480253-03',
'VA8480253-03',
'AD8575702-03',
'OF8472384-01',
'OF8472384-01',
'OF8472384-01',
'OF8472384-01',
'OF8472384-01',
'OF8472384-01',
'OF8472384-01',
'OF8472384-01',
'OF8472384-01',
'OF8472384-01',
'MB8474023-03',
'MB8474023-03',
'MB8474023-03',
'MB8474023-03',
'MB8474023-03',
'MB8474023-03',
'MB8474023-03',
'MB8474023-03',
'MB8474023-03',
'VB8490544-02',
'VB8490544-02',
'VB8490544-02',
'VB8490544-02',
'VB8490544-02',
'VB8490544-02',
'VB8490544-02',
'VB8490544-02',
'VB8490544-02',
'VJ8490890-01',
'VJ8490890-01',
'VJ8490890-01',
'VJ8490890-01',
'VJ8490890-01',
'VJ8490890-01',
'VJ8490890-01',
'VB8490379-04',
'VB8490379-04',
'VB8490379-04',
'VB8490379-04',
'VB8490379-04',
'VB8490379-04',
'VB8490379-04',
'VB8490379-04',
'VB8490379-04',
'PZ8431891-01',
'PZ8431891-01',
'PZ8431891-01',
'PZ8431891-01',
'PZ8431891-01',
'PZ8431891-01',
'PZ8431891-01',
'PZ8431891-01',
'PZ8431891-01',
'OV8452045-02',
'OV8452045-02',
'OV8452045-02',
'OV8452045-02',
'OV8452045-02',
'OV8452045-02',
'OV8452045-02',
'OV8452045-02',
'OV8452045-02',
'OV8452045-02',
'PZ8431833-01',
'PZ8431833-01',
'PZ8431833-01',
'PZ8431833-01',
'PZ8431833-01',
'PZ8431833-01',
'PZ8431833-01',
'PZ8431833-01',
'PZ8431833-01',
'PZ8431833-01',
'MB8473095-03',
'OV8452058-01',
'OV8452058-01',
'OV8452058-01',
'OV8452058-01',
'OV8452058-01',
'OV8452058-01',
'OV8452058-01',
'OV8452058-01',
'OV8452058-01',
'OV8452058-01',
'OV8452058-01',
'OV8452058-01',
'OV8452058-01',
'OV8452058-01',
'AV8651927-01',
'AV8651927-01',
'AV8651927-01',
'AV8651927-01',
'AV8651927-01',
'AV8651927-01',
'AV8651927-01',
'AV8651927-01',
'AV8651927-01',
'OH8459637-03',
'OH8459637-03',
'OH8459637-03',
'OH8459637-03',
'OH8459637-03',
'OH8459637-03',
'OH8459637-03',
'OH8459637-03',
'OH8459637-03',
'OH8459637-03',
'OH8458735-02',
'OH8458735-02',
'OH8458735-02',
'OH8458735-02',
'OH8458735-02',
'OH8458735-02',
'OH8458735-02',
'OH8458735-02',
'OH8458735-02',
'OH8458735-02',
'OH8458735-02',
'OH8458735-02',
'OH8458735-02',
'OH8458735-02',
'VQ8463458-01',
'VQ8463458-01',
'VQ8463458-01',
'VQ8463458-01',
'VQ8463458-01',
'VQ8463458-01',
'VQ8463458-01',
'VQ8463458-01',
'VQ8463458-01',
'VA8482963-02',
'VA8482963-02',
'VA8482963-02',
'VA8482963-02',
'VA8482963-02',
'VA8482963-02',
'VA8482963-02',
'VA8482963-02',
'VI8476089-01',
'VQ8465731-02',
'VQ8465731-02',
'VQ8465731-02',
'VQ8465731-02',
'VQ8465731-02',
'VQ8465731-02',
'VQ8465731-02',
'VQ8465731-02',
'VQ8465744-01',
'VQ8465744-01',
'VQ8465744-01',
'VQ8465744-01',
'VQ8465744-01',
'VQ8465744-01',
'VQ8465744-01',
'VQ8465744-01',
'VA8482989-01',
'VA8482989-01',
'VA8482989-01',
'VA8482989-01',
'VA8482989-01',
'VA8482989-01',
'VA8482989-01',
'VA8482989-01',
'MD8489678-01',
'MD8489678-01',
'MD8489678-01',
'MD8489678-01',
'MD8489678-01',
'MD8489678-01',
'MD8489678-01',
'MD8489678-01',
'MD8489678-01',
'MD8489678-01',
'VJ8491789-01',
'VJ8491789-01',
'VJ8491789-01',
'VJ8491789-01',
'VJ8491789-01',
'VJ8491789-01',
'VJ8491789-01',
'VJ8491789-01',
'VJ8491789-01',
'VA8478472-01',
'VA8478472-01',
'VA8478472-01',
'VA8478472-01',
'VA8478472-01',
'VA8478472-01',
'VA8478472-01',
'OH8458557-02',
'OH8458557-02',
'OH8458557-02',
'OH8458557-02',
'OH8458557-02',
'OH8458557-02',
'OH8458557-02',
'OH8458557-02',
'OH8458557-02',
'OH8458557-02',
'OH8458557-02',
'OH8458557-02',
'OH8458557-02',
'OH8458557-02',
'VQ8466028-03',
'MD8489775-02',
'MD8489775-02',
'MD8489775-02',
'MD8489775-02',
'MD8489775-02',
'MD8489775-02',
'MD8489775-02',
'MD8489775-02',
'MD8489775-02',
'MD8489775-02',
'VJ8490900-01',
'VJ8490900-01',
'VJ8490900-01',
'VJ8490900-01',
'VJ8490900-01',
'VJ8490900-01',
'VJ8490900-01',
'VJ8490900-01',
'CX8447982-01',
'CX8447982-01',
'CX8447982-01',
'CX8447982-01',
'CX8447982-01',
'CX8447982-01',
'CX8447982-01',
'AD8579025-03',
'AD8579025-03',
'AD8579025-03',
'AD8579025-03',
'AD8579025-03',
'AD8579025-03',
'AD8579025-03',
'AD8579025-03',
'AD8579025-03',
'VQ8464965-01',
'VQ8464965-01',
'VQ8464965-01',
'VQ8464965-01',
'VQ8464965-01',
'VQ8464965-01',
'MD8489681-01',
'MD8489681-01',
'MD8489681-01',
'MD8489681-01',
'MD8489681-01',
'MD8489681-01',
'MD8489681-01',
'MD8489681-01',
'MD8489704-01',
'MD8489704-01',
'MD8489704-01',
'MD8489704-01',
'MD8489704-01',
'MD8489704-01',
'MD8489704-01',
'MD8489704-01',
'VQ8464965-02',
'VQ8464965-02',
'VQ8464965-02',
'VQ8464965-02',
'VQ8464965-02',
'VQ8464965-02',
'MD8489717-01',
'MD8489717-01',
'MD8489717-01',
'MD8489717-01',
'MD8489717-01',
'MD8489717-01',
'MD8489717-01',
'MD8489717-01',
'MD8489717-01',
'MD8489717-01',
'AD8579012-02',
'AD8579012-02',
'AD8579012-02',
'AD8579012-02',
'AD8579012-02',
'AD8579012-02',
'AD8579012-02',
'AD8579012-02',
'VL8442314-01',
'VL8442314-01',
'VL8442314-01',
'VL8442314-01',
'VL8442314-01',
'VL8442314-01',
'VL8442314-01',
'MI8491543-01',
'OV8452278-01',
'OV8452278-01',
'OV8452278-01',
'OV8452278-01',
'OV8452278-01',
'OV8452278-01',
'OV8452278-01',
'OV8452278-01',
'OV8452278-01',
'OV8452278-01',
'OV8452278-01',
'OV8452278-01',
'OV8452278-01',
'OV8452278-01',
'OI8517472-02',
'OI8517472-02',
'OI8517472-02',
'OI8517472-02',
'OI8517472-02',
'OI8517472-02',
'OI8517472-02',
'OI8517472-02',
'OI8517472-02',
'OI8517472-02',
'OV8452265-01',
'OV8452265-01',
'OV8452265-01',
'OV8452265-01',
'OV8452265-01',
'OV8452265-01',
'OV8452265-01',
'OV8452265-01',
'OV8452265-01',
'OV8452265-01',
'OI8417485-02',
'OI8417485-01',
'SN8467522-01',
'SN8467522-01',
'SN8467522-01',
'SN8467522-01',
'SN8467522-01',
'SN8467522-01',
'SN8467522-01',
'SN8467522-01',
'SN8467522-01',
'SN8467522-01',
'SN8467522-01',
'MA8467690-01',
'MA8467690-01',
'MA8467690-01',
'MA8467690-01',
'MA8467690-01',
'MA8467690-01',
'MA8467690-01',
'MA8467690-01',
'MA8467690-01',
'MA8467690-01',
'MA8467700-01',
'MA8467700-01',
'MA8467700-01',
'MA8467700-01',
'MA8467700-01',
'MA8467700-01',
'MA8467700-01',
'MA8467700-01',
'OI8518002-01',
'OI8518002-01',
'OI8518002-01',
'OI8518002-01',
'OI8518002-01',
'OI8518002-01',
'OI8518002-01',
'OI8518002-01',
'OI8518002-01',
'OI8518002-01',
'AV8645360-01',
'AV8645360-01',
'AV8645360-01',
'AV8645360-01',
'AV8645360-01',
'AV8645360-01',
'AV8645360-01',
'AV8645360-01',
'AV8645360-01',
'AV8645360-01',
'AV8645360-01',
'BG8468738-01',
'BG8468738-01',
'BG8468738-01',
'BG8468738-01',
'BG8468738-01',
'BG8468738-01',
'BG8468738-01',
'AV8649065-01',
'AV8649065-01',
'AV8649065-01',
'AV8649065-01',
'AV8649065-01',
'AV8649065-01',
'AV8649065-01',
'AV8649065-01',
'AV8649065-01') 