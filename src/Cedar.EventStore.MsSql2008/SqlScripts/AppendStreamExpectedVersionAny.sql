/*SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;*/
BEGIN TRANSACTION AppendStream;
    DECLARE @streamIdInternal AS INT;
    DECLARE @latestStreamVersion AS INT;

     SELECT @streamIdInternal = dbo.Streams.IdInternal
       FROM dbo.Streams
      WHERE dbo.Streams.Id = @streamId;

         IF @streamIdInternal IS NULL
            BEGIN
                INSERT INTO dbo.Streams (Id, IdOriginal) VALUES (@streamId, @streamIdOriginal);
                SELECT @streamIdInternal = SCOPE_IDENTITY();

                INSERT INTO dbo.Events (StreamIdInternal, StreamVersion, Id, Created, [Type], JsonData, JsonMetadata)
                 SELECT @streamIdInternal,
                        StreamVersion,
                        Id,
                        Created,
                        [Type],
                        JsonData,
                        JsonMetadata
                   FROM @newEvents;
            END
       ELSE
           BEGIN
                 SELECT TOP(1)
                         @latestStreamVersion = dbo.Events.StreamVersion
                    FROM dbo.Events
                   WHERE dbo.Events.StreamIDInternal = @streamIdInternal
                ORDER BY dbo.Events.Ordinal DESC;

            INSERT INTO dbo.Events (StreamIdInternal, StreamVersion, Id, Created, [Type], JsonData, JsonMetadata)
                 SELECT @streamIdInternal,
                        StreamVersion + @latestStreamVersion + 1,
                        Id,
                        Created,
                        [Type],
                        JsonData,
                        JsonMetadata
                   FROM @newEvents;
           END
COMMIT TRANSACTION AppendStream;
