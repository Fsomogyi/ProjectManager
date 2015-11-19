BEGIN TRAN

INSERT INTO TaskState
VALUES
('New'),
('Active'),
('Done'),
('Deleted')

COMMIT